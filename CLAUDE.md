# CvPortal

Yazılım mühendisi CV / portfolyo uygulaması; "fişi tak çalıştır" olarak satılacak ürün.
.NET 10, ASP.NET Core MVC, Clean Architecture, EF Core + SQL Server.
Kullanıcıyla Türkçe iletişim kurulur; UI metinleri, doğrulama mesajları ve kod yorumları Türkçedir.

## Komutlar

```bash
dotnet build CvPortal.slnx
dotnet test CvPortal.slnx
dotnet run --project src/CvPortal.Web --launch-profile https   # https://localhost:7060 (SQL: .\SQLEXPRESS)
dotnet tool restore                                             # yerel dotnet-ef aracı
dotnet publish src/CvPortal.Web -p:PublishProfile=SatisPaketi   # artifacts/CvPortal
```

## Katmanlar ve bağımlılık yönü

`Web → Application ← Infrastructure`, hepsi `Domain`'e bağlı. Domain hiçbir projeye bağlı değildir.

- **Domain** — entity'ler (`BaseEntity`, `IOrderable`, `IPublishable`) ve enum'lar (`[Display(Name)]` ile Türkçe adlar).
  `AdminUser` ve `SiteSettings` (tekil kayıt: API anahtarı, kurulum zamanı) de buradadır.
- **Application** — DTO'lar (DataAnnotations + `ValidationMessages`), `MappingExtensions` (elle yazılmış ToDto/Apply),
  servis arayüzleri ve implementasyonları. İçerik servisleri `CrudServiceBase<TEntity, TDto>`'dan türer.
  Zaman için `TimeProvider` enjekte edilir; `DateTime.Now` kullanılmaz. Filtreler `Expression` olarak repository'ye verilir.
- **Infrastructure** — `CvPortalDbContext` + `Persistence/Configurations` (kolon uzunlukları DTO'larla aynı, enum → string,
  `List<string>` → JSON), `EfRepository<T>`, `DatabaseInitializer` (açılışta migrate + demo seed), `Persistence/Migrations`.
  `Database:Provider = InMemory` ise `InMemoryRepository<T>` + `MockDataProvider` kullanılır.
- **Web** — public site, Admin Area (`CrudAdminController<TDto>` + `Index.cshtml`/`Form.cshtml`, `SettingsController`),
  `/setup` kurulum sihirbazı (`Setup/`), REST API (`[ApiKeyAuthorize]`, anahtar DB'de). Yönetici girişi `AdminAuthService`
  (PasswordHasher, PBKDF2). `reset-password` komut satırı aracı `Setup/AdminCommandLine.cs`.

## Yeni içerik tipi eklerken

1. Domain'e entity (`IOrderable`, `IPublishable`), Application'a DTO (`IHasId`) + mapping + `I...Service : ICrudService<TDto>` + servis.
2. `Persistence/Configurations/EntityConfigurations.cs`'e yapılandırma, `CvPortalDbContext`'e `DbSet`,
   `ContentResetService`'e silme adımı, `MockDataProvider` + `DatabaseInitializer`'a örnek veri.
3. Migration ekle ve `database/02_CreateSchema.sql`'i yeniden üret (README → Migration).
4. DI kayıtları: `Application/DependencyInjection.cs`, `Infrastructure/DependencyInjection.cs` (InMemory bölümü).
5. Admin: `ContentControllers.cs`'e controller, `Areas/Admin/Views/<Ad>/Index.cshtml` + `Form.cshtml`, `_AdminLayout` menüsüne link.
6. API: `ContentApiControllers.cs`'e `[Route("api/v1/...")]` controller.
7. `tests/CvPortal.Tests` altına servis testi (EF davranışı için `DatabaseTests` SQLite kalıbını kullan).

## Kurallar

- Model değişikliği = yeni migration + `database/02_CreateSchema.sql` güncellemesi. Mevcut migration'ları düzenleme.
- Sırlar (şifre, API anahtarı) `appsettings.json`'a yazılmaz; DB'de tutulur ve admin panel → Ayarlar'dan yönetilir.
- Tasarım token'ları `wwwroot/css/site.css` (public) ve `admin.css` (admin) içindeki CSS değişkenleridir; açık/koyu tema `data-bs-theme` ile.
- Admin POST formları `AdminControllerBase` üzerindeki `[AutoValidateAntiforgeryToken]` ile korunur; silme formlarına `data-confirm` ekle.
- Müşteriye dönük değişikliklerde `KURULUM.md`'yi de güncelle.
- Dosya yüklemeleri `Web/Uploads` (`MediaUploader` doğrulama, `MediaChangeSet` commit/rollback) üzerinden yapılır;
  dosyalar `App_Data/uploads`'a yazılır ve `/media` altında sunulur. wwwroot'a çalışma zamanında dosya yazma.
