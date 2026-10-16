# CvPortal

Yazılım mühendisleri için **admin panel** ve **REST API** ile yönetilebilen kişisel CV / portfolyo uygulaması.
.NET 10 · ASP.NET Core MVC · Clean Architecture · EF Core + SQL Server.

> Müşteri / son kullanıcı kurulumu için: **[KURULUM.md](KURULUM.md)**

## Geliştirme ortamında çalıştırma

Gereksinimler: .NET 10 SDK, SQL Server (Express veya LocalDB).

1. `src/CvPortal.Web/appsettings.json` → `ConnectionStrings:DefaultConnection` değerini kontrol edin (varsayılan: `.\SQLEXPRESS`).
2. Çalıştırın — veritabanı, tablolar ve örnek içerik ilk açılışta otomatik oluşur:

```bash
dotnet run --project src/CvPortal.Web
```

3. Tarayıcıda `/setup` → ilk yönetici hesabını oluşturun.

Veritabanı olmadan denemek için `Database:Provider` değerini `InMemory` yapın.

| Adres | Açıklama |
|---|---|
| `/` | Herkese açık CV sayfası |
| `/setup` | İlk kurulum sihirbazı (yalnızca hiç yönetici yokken) |
| `/Admin` | Yönetim paneli |
| `/scalar/v1` | API dokümantasyonu (Scalar) |
| `/health` | Sağlık kontrolü (veritabanı bağlantısı dahil) |

## Proje yapısı

```
CvPortal.slnx
├─ src/
│  ├─ CvPortal.Domain          Entity'ler, enum'lar (bağımlılıksız)
│  ├─ CvPortal.Application     DTO'lar, doğrulama, servis arayüzleri ve iş kuralları
│  ├─ CvPortal.Infrastructure  EF Core DbContext, EfRepository, migration'lar, InMemoryRepository, MockDataProvider
│  └─ CvPortal.Web             MVC site, Admin Area, kurulum sihirbazı, REST API, cookie auth
├─ database/                   SSMS için SQL scriptleri (oluşturma, şema, sıfırlama, yedek)
└─ tests/
   └─ CvPortal.Tests           xUnit testleri (servisler, EF Core/SQLite, kurulum, güvenlik)
```

### Veri katmanı

- `IRepository<T>` iki implementasyona sahiptir: `EfRepository<T>` (SQL Server) ve `InMemoryRepository<T>` (demo/test).
- `DatabaseInitializer` açılışta migration'ları uygular ve boş veritabanına örnek içerik ekler.
- Kolon uzunlukları DTO doğrulama kurallarıyla aynıdır; enum'lar metin, listeler JSON olarak saklanır.

| Tablo | İçerik |
|---|---|
| `Profiles` | Tekil profil kaydı |
| `Experiences`, `Educations`, `Skills`, `Projects`, `Certificates`, `Languages` | CV içeriği |
| `ContactMessages` | İletişim formu mesajları |
| `AdminUsers` | Yönetici hesapları (PBKDF2 şifre hash'i) |
| `SiteSettings` | API anahtarı, API açık/kapalı, kurulum zamanı |

### Migration

Model değiştiğinde (proje kökünde; `dotnet-ef` yerel araç olarak `dotnet-tools.json` içinde tanımlı):

```bash
dotnet tool restore
dotnet ef migrations add <Ad> --project src/CvPortal.Infrastructure --startup-project src/CvPortal.Web --output-dir Persistence/Migrations
dotnet ef migrations script --idempotent --project src/CvPortal.Infrastructure --startup-project src/CvPortal.Web --output database/02_CreateSchema.sql
```

`02_CreateSchema.sql` yeniden üretildikten sonra dosyanın başındaki açıklama ve `USE [CvPortal];` satırını geri ekleyin.

### Servisler

| Servis | Görev |
|---|---|
| `IProfileService` | Tekil profil bilgisi (ad, ünvan, iletişim, sosyal linkler) |
| `IExperienceService` … `ILanguageService` | CV içeriği (CRUD, görünürlük, sıralama) |
| `IContactMessageService` | İletişim formu mesajları (okundu/okunmadı) |
| `ICvService` | Yayındaki tüm içeriği tek modelde toplar (public sayfa + API) |
| `IDashboardService` | Admin özet istatistikleri, profil doluluk oranı |
| `ISiteSettingsService` | API anahtarı üretme/doğrulama, kurulum durumu |
| `IContentResetService` | Tüm CV içeriğini temizleme |
| `IAdminAuthService` (Web) | Yönetici hesapları, giriş, şifre değiştirme/sıfırlama |
| `ISetupService` (Web) | İlk kurulum sihirbazı |

## Admin panel

Dashboard · Profil · Deneyimler · Eğitim · Yetenekler · Projeler · Sertifikalar · Diller · Mesajlar · **Ayarlar**

- Ekle / düzenle / sil, sitede gizle/göster, sıralama; sunucu + istemci tarafı Türkçe doğrulama
- Ayarlar: hesap bilgileri, şifre değiştirme, REST API aç/kapat, API anahtarı yenileme, tüm içeriği temizleme
- Dosya yükleme: profil fotoğrafı, CV PDF'i ve proje görselleri. Dosyalar `App_Data/uploads` altında saklanır ve
  `/media/...` adresinden yayınlanır (`IFileStorage` → `LocalFileStorage`). Tür, dosya imzasıyla doğrulanır
  (JPG/PNG/WebP/GIF ≤ 5 MB, PDF ≤ 10 MB; SVG kabul edilmez). `MediaChangeSet` eski dosyayı yalnızca kayıt
  başarılı olunca siler, hata olursa yeni dosyayı geri alır.
- Güvenlik: PBKDF2 şifre hash'i, giriş denemesi sınırı (IP başına 5 dk'da 10), antiforgery, kalıcı Data Protection anahtarları

## REST API

Yönetim uç noktaları `X-Api-Key` başlığı ister. API varsayılan olarak kapalıdır; admin panel → **Ayarlar** ekranından açılır
ve anahtar oradan alınır. Tarayıcıdan çağıracak paneller için `Api:AllowedOrigins` (CORS) listesini doldurun.

| Metot | Uç nokta | Not |
|---|---|---|
| GET | `/api/v1/cv` | Herkese açık, yayındaki CV |
| GET/PUT | `/api/v1/profile` | |
| GET/POST | `/api/v1/{experiences,educations,skills,projects,certificates,languages}` | |
| GET/PUT/DELETE | `/api/v1/{kaynak}/{id}` | |
| PATCH | `/api/v1/{kaynak}/{id}/visibility` | Görünürlüğü değiştir |
| PATCH | `/api/v1/projects/{id}/featured` | Öne çıkar |
| GET | `/api/v1/skills/grouped` | Kategoriye göre |
| GET | `/api/v1/messages` · `PATCH /{id}/read` · `DELETE /{id}` | |
| POST | `/api/v1/messages` | Herkese açık iletişim formu (rate limit: 5/dk) |
| GET | `/api/v1/dashboard` | Özet istatistikler |

## Satış paketi oluşturma

```bash
dotnet publish src/CvPortal.Web -p:PublishProfile=SatisPaketi
```

Çıktı: `artifacts/CvPortal` (~32 MB, Windows x64, .NET 10 runtime gerektirir). Bu klasörü `database/` scriptleri,
`KURULUM.md` ve `docs/CvPortal-Kullanim-Kilavuzu.pdf` ile birlikte zip'leyip teslim edin. Rider / Visual Studio'da: *CvPortal.Web → Publish → SatisPaketi*.

## Testler

```bash
dotnet test
```
