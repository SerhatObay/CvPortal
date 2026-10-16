using System.Text;
using CvPortal.Infrastructure.Storage;
using CvPortal.Web.Uploads;
using Microsoft.AspNetCore.Http;

namespace CvPortal.Tests;

public sealed class UploadTests : IDisposable
{
    private static readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 13, 0x49, 0x48, 0x44, 0x52];
    private static readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0, 16, 0x4A, 0x46, 0x49, 0x46, 0, 1, 1, 0, 0, 1];
    private static readonly byte[] Pdf = Encoding.ASCII.GetBytes("%PDF-1.7\n%âãÏÓ\n1 0 obj");
    private static readonly byte[] Svg = Encoding.UTF8.GetBytes("<svg xmlns='http://www.w3.org/2000/svg'><script>alert(1)</script></svg>");

    private readonly string _root = Path.Combine(Path.GetTempPath(), "cvportal-tests", Guid.NewGuid().ToString("N"));
    private readonly LocalFileStorage _storage;
    private readonly MediaUploader _uploader;

    public UploadTests()
    {
        _storage = new LocalFileStorage(_root);
        _uploader = new MediaUploader(_storage);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root)) Directory.Delete(_root, recursive: true);
    }

    private static FormFile File(byte[] bytes, string name) =>
        new(new MemoryStream(bytes), 0, bytes.Length, "file", name) { Headers = new HeaderDictionary() };

    private string DiskPath(string url) => Path.Combine(_root, url["/media/".Length..].Replace('/', Path.DirectorySeparatorChar));

    [Fact]
    public async Task Image_IsStoredWithRandomNameAndDetectedExtension()
    {
        // Uzantı .gif olsa da içerik PNG: kaydedilen dosya .png olmalı.
        var result = await _uploader.SaveAsync(File(Png, "foto.gif"), UploadKind.Image, "avatar");

        Assert.True(result.Succeeded);
        Assert.Matches(@"^/media/avatar/[a-f0-9]{32}\.png$", result.Url);
        Assert.True(System.IO.File.Exists(DiskPath(result.Url!)));
    }

    [Theory]
    [MemberData(nameof(RejectedImages))]
    public async Task Image_RejectsNonImageContent(byte[] content, string name)
    {
        var result = await _uploader.SaveAsync(File(content, name), UploadKind.Image, "avatar");

        Assert.False(result.Succeeded);
        Assert.False(Directory.Exists(Path.Combine(_root, "avatar")) && Directory.EnumerateFiles(Path.Combine(_root, "avatar")).Any());
    }

    public static TheoryData<byte[], string> RejectedImages => new()
    {
        { Svg, "logo.svg" },
        { Pdf, "foto.jpg" },
        { Encoding.ASCII.GetBytes("MZ\x90\0 sahte exe"), "foto.png" }
    };

    [Fact]
    public async Task Pdf_AcceptsOnlyPdf()
    {
        Assert.True((await _uploader.SaveAsync(File(Pdf, "cv.pdf"), UploadKind.Pdf, "cv")).Succeeded);
        Assert.False((await _uploader.SaveAsync(File(Jpeg, "cv.pdf"), UploadKind.Pdf, "cv")).Succeeded);
    }

    [Fact]
    public async Task Image_RejectsTooLargeFile()
    {
        var big = new byte[MediaUploader.MaxImageBytes + 1];
        Png.CopyTo(big, 0);

        var result = await _uploader.SaveAsync(File(big, "buyuk.png"), UploadKind.Image, "avatar");

        Assert.False(result.Succeeded);
        Assert.Contains("5 MB", result.Error);
    }

    [Theory]
    [InlineData("/media/../appsettings.json")]
    [InlineData("/media/avatar/../../secret.txt")]
    [InlineData("/images/avatar.svg")]
    [InlineData("https://example.com/media/avatar/a.png")]
    public void Storage_DoesNotManageForeignOrTraversalPaths(string url)
        => Assert.False(_storage.IsManaged(url));

    [Fact]
    public async Task ChangeSet_Commit_DeletesReplacedFile_Rollback_DeletesNewFile()
    {
        var old = (await _uploader.SaveAsync(File(Png, "eski.png"), UploadKind.Image, "projects")).Url!;

        // Başarılı kayıt: yeni dosya kalır, eski dosya silinir.
        var committed = new MediaChangeSet(_uploader);
        var replaced = await committed.ApplyAsync(File(Jpeg, "yeni.jpg"), false, old, old, UploadKind.Image, "projects");
        await committed.CommitAsync();

        Assert.False(System.IO.File.Exists(DiskPath(old)));
        Assert.True(System.IO.File.Exists(DiskPath(replaced.Url!)));

        // Başarısız kayıt: bu istekte yüklenen dosya geri alınır, mevcut dosyaya dokunulmaz.
        var rolledBack = new MediaChangeSet(_uploader);
        var attempt = await rolledBack.ApplyAsync(File(Png, "deneme.png"), false, replaced.Url, replaced.Url, UploadKind.Image, "projects");
        await rolledBack.RollbackAsync();

        Assert.False(System.IO.File.Exists(DiskPath(attempt.Url!)));
        Assert.True(System.IO.File.Exists(DiskPath(replaced.Url!)));
    }

    [Fact]
    public async Task ChangeSet_Remove_ReturnsNullAndDeletesOnCommit()
    {
        var current = (await _uploader.SaveAsync(File(Pdf, "cv.pdf"), UploadKind.Pdf, "cv")).Url!;
        var set = new MediaChangeSet(_uploader);

        var result = await set.ApplyAsync(null, remove: true, current, current, UploadKind.Pdf, "cv");
        await set.CommitAsync();

        Assert.Null(result.Url);
        Assert.False(System.IO.File.Exists(DiskPath(current)));
    }
}
