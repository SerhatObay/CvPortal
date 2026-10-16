namespace CvPortal.Application.Abstractions;

/// <summary>
/// Yüklenen dosyaların (profil fotoğrafı, CV PDF, proje görseli) saklandığı yer.
/// Varsayılan implementasyon dosyaları sunucu diskinde tutar; ileride bulut depolama ile değiştirilebilir.
/// </summary>
public interface IFileStorage
{
    /// <summary>Dosyayı rastgele bir adla kaydeder ve sitede kullanılacak adresi döner (ör. "/media/avatar/3f2a….jpg").</summary>
    Task<string> SaveAsync(Stream content, string folder, string extension, CancellationToken ct = default);

    /// <summary>Adres bu depolamaya aitse dosyayı siler; harici URL'lere ve bulunamayan dosyalara dokunmaz.</summary>
    Task DeleteAsync(string? url, CancellationToken ct = default);

    /// <summary>Adres bu depolama tarafından üretildiyse true.</summary>
    bool IsManaged(string? url);
}
