using CvPortal.Application.Abstractions;

namespace CvPortal.Web.Uploads;

public enum UploadKind
{
    /// <summary>JPG, PNG, WebP, GIF — en fazla 5 MB.</summary>
    Image,

    /// <summary>PDF — en fazla 10 MB.</summary>
    Pdf
}

public sealed record UploadResult(string? Url, string? Error)
{
    public bool Succeeded => Error is null;
    public static UploadResult Fail(string error) => new(null, error);
}

/// <summary>
/// Admin panelden gelen dosyaları doğrular ve depolar. Dosya türü uzantıya değil,
/// dosyanın ilk baytlarına (imza) bakılarak belirlenir; SVG gibi script içerebilen türler kabul edilmez.
/// </summary>
public sealed class MediaUploader(IFileStorage storage)
{
    public const long MaxImageBytes = 5 * 1024 * 1024;
    public const long MaxPdfBytes = 10 * 1024 * 1024;

    public const string ImageAccept = ".jpg,.jpeg,.png,.webp,.gif,image/jpeg,image/png,image/webp,image/gif";
    public const string PdfAccept = ".pdf,application/pdf";

    public async Task<UploadResult> SaveAsync(IFormFile file, UploadKind kind, string folder, CancellationToken ct = default)
    {
        var maxBytes = kind == UploadKind.Image ? MaxImageBytes : MaxPdfBytes;
        if (file.Length == 0) return UploadResult.Fail("Seçilen dosya boş.");
        if (file.Length > maxBytes) return UploadResult.Fail($"Dosya en fazla {maxBytes / 1024 / 1024} MB olabilir.");

        string? extension;
        await using (var probe = file.OpenReadStream())
        {
            var header = new byte[16];
            var read = await probe.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, ct);
            extension = DetectExtension(header.AsSpan(0, read));
        }

        var allowed = kind == UploadKind.Image ? extension is ".jpg" or ".png" or ".webp" or ".gif" : extension == ".pdf";
        if (!allowed)
        {
            return UploadResult.Fail(kind == UploadKind.Image
                ? "Yalnızca JPG, PNG, WebP veya GIF görselleri yüklenebilir."
                : "Yalnızca PDF dosyası yüklenebilir.");
        }

        await using var content = file.OpenReadStream();
        return new UploadResult(await storage.SaveAsync(content, folder, extension!, ct), null);
    }

    public Task DeleteAsync(string? url, CancellationToken ct = default) => storage.DeleteAsync(url, ct);

    /// <summary>Dosya imzasından (magic bytes) gerçek türü belirler.</summary>
    public static string? DetectExtension(ReadOnlySpan<byte> h)
    {
        if (h.Length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF) return ".jpg";
        if (h.Length >= 8 && h[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) return ".png";
        if (h.Length >= 6 && (h[..6].SequenceEqual("GIF87a"u8) || h[..6].SequenceEqual("GIF89a"u8))) return ".gif";
        if (h.Length >= 12 && h[..4].SequenceEqual("RIFF"u8) && h[8..12].SequenceEqual("WEBP"u8)) return ".webp";
        if (h.Length >= 5 && h[..5].SequenceEqual("%PDF-"u8)) return ".pdf";
        return null;
    }
}

/// <summary>
/// Bir form kaydındaki dosya değişikliklerini toplar. Eski dosyalar ancak kayıt başarılı olunca (Commit) silinir;
/// kayıt başarısız olursa yeni yüklenen dosyalar geri alınır (Rollback). Böylece veritabanı ile disk tutarlı kalır.
/// </summary>
public sealed class MediaChangeSet(MediaUploader uploader)
{
    private readonly List<string> _uploaded = [];
    private readonly List<string> _replaced = [];

    /// <summary>
    /// Alan için yeni değeri belirler: yeni dosya yüklendiyse onun adresi, "kaldır" seçildiyse null,
    /// aksi hâlde formdan gelen adres. Hata olursa <see cref="UploadResult.Error"/> dolu döner.
    /// </summary>
    public async Task<UploadResult> ApplyAsync(IFormFile? file, bool remove, string? currentUrl, string? postedUrl,
        UploadKind kind, string folder, CancellationToken ct = default)
    {
        if (file is { Length: > 0 })
        {
            var result = await uploader.SaveAsync(file, kind, folder, ct);
            if (!result.Succeeded) return result;

            _uploaded.Add(result.Url!);
            MarkReplaced(currentUrl);
            return result;
        }

        if (remove)
        {
            MarkReplaced(currentUrl);
            return new UploadResult(null, null);
        }

        var posted = string.IsNullOrWhiteSpace(postedUrl) ? null : postedUrl.Trim();
        if (!string.Equals(posted, currentUrl, StringComparison.OrdinalIgnoreCase)) MarkReplaced(currentUrl);
        return new UploadResult(posted, null);
    }

    /// <summary>Kayıt başarılı: artık kullanılmayan eski dosyaları sil.</summary>
    public async Task CommitAsync(CancellationToken ct = default)
    {
        foreach (var url in _replaced) await uploader.DeleteAsync(url, ct);
        _replaced.Clear();
        _uploaded.Clear();
    }

    /// <summary>Kayıt başarısız: bu istekte yüklenen dosyaları sil.</summary>
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        foreach (var url in _uploaded) await uploader.DeleteAsync(url, ct);
        _replaced.Clear();
        _uploaded.Clear();
    }

    private void MarkReplaced(string? url)
    {
        if (!string.IsNullOrWhiteSpace(url)) _replaced.Add(url);
    }
}
