using System.Text.RegularExpressions;
using CvPortal.Application.Abstractions;

namespace CvPortal.Infrastructure.Storage;

/// <summary>
/// Dosyaları sunucu diskinde saklar (varsayılan: App_Data/uploads) ve "/media/..." adresinden yayınlanmalarını sağlar.
/// Dosya adları her zaman rastgele üretilir; kullanıcının verdiği ad ve klasör yolu kullanılmaz.
/// </summary>
public sealed partial class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;
    private readonly string _requestPath;

    public LocalFileStorage(string rootPath, string requestPath = "/media")
    {
        _rootPath = Path.GetFullPath(rootPath);
        _requestPath = "/" + requestPath.Trim('/');
        Directory.CreateDirectory(_rootPath);
    }

    public string RootPath => _rootPath;

    public async Task<string> SaveAsync(Stream content, string folder, string extension, CancellationToken ct = default)
    {
        if (!SafeSegment().IsMatch(folder)) throw new ArgumentException("Geçersiz klasör adı.", nameof(folder));
        if (!SafeExtension().IsMatch(extension)) throw new ArgumentException("Geçersiz dosya uzantısı.", nameof(extension));

        var directory = Path.Combine(_rootPath, folder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        await using (var file = new FileStream(Path.Combine(directory, fileName), FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await content.CopyToAsync(file, ct);
        }

        return $"{_requestPath}/{folder}/{fileName}";
    }

    public Task DeleteAsync(string? url, CancellationToken ct = default)
    {
        if (TryResolve(url, out var fullPath) && File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public bool IsManaged(string? url) => TryResolve(url, out _);

    /// <summary>"/media/klasör/dosya.uzantı" biçimindeki adresi diskteki yola çevirir; kök dışına çıkmaya izin vermez.</summary>
    private bool TryResolve(string? url, out string fullPath)
    {
        fullPath = string.Empty;
        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(_requestPath + "/", StringComparison.OrdinalIgnoreCase))
            return false;

        var parts = url[(_requestPath.Length + 1)..].Split('/');
        if (parts.Length != 2 || !SafeSegment().IsMatch(parts[0]) || !SafeFileName().IsMatch(parts[1]))
            return false;

        fullPath = Path.GetFullPath(Path.Combine(_rootPath, parts[0], parts[1]));
        return fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex("^[a-z0-9-]{1,40}$")]
    private static partial Regex SafeSegment();

    [GeneratedRegex(@"^\.[a-zA-Z0-9]{2,5}$")]
    private static partial Regex SafeExtension();

    [GeneratedRegex(@"^[a-f0-9]{32}\.[a-z0-9]{2,5}$")]
    private static partial Regex SafeFileName();
}
