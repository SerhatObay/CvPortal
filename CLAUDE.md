# CvPortal

Yazılım mühendisi CV / portfolyo uygulaması. .NET 10, ASP.NET Core MVC, Clean Architecture, mock (in-memory) veri.
Kullanıcıyla Türkçe iletişim kurulur; UI metinleri, doğrulama mesajları ve kod yorumları Türkçedir.

## Komutlar

```bash
dotnet build CvPortal.slnx
dotnet test CvPortal.slnx
dotnet run --project src/CvPortal.Web --launch-profile https   # https://localhost:7060
```

## Katmanlar ve bağımlılık yönü

`Web → Application ← Infrastructure`, hepsi `Domain`'e bağlı. Domain hiçbir projeye bağlı değildir.

- **Domain** — entity'ler (`BaseEntity`, `IOrderable`, `IPublishable`) ve enum'lar (`[Display(Name)]` ile Türkçe adlar).
- **Application** — DTO'lar (DataAnnotations + `ValidationMessages`), `MappingExtensions` (elle yazılmış ToDto/Apply),
  servis arayüzleri ve implementasyonları. İçerik servisleri `CrudServiceBase<TEntity, TDto>`'dan türer.
  Zaman için `TimeProvider` enjekte edilir; `DateTime.Now` kullanılmaz.
- **Infrastructure** — `InMemoryRepository<T>` (singleton, thread-safe) ve `MockDataProvider`. Gerçek DB'ye geçişte yalnızca
  `IRepository<T>` implementasyonu değişir.
- **Web** — public site (`HomeController` + `Views/Home/Sections/_*.cshtml`), Admin Area (cookie auth,
  `CrudAdminController<TDto>` + her içerik için `Index.cshtml`/`Form.cshtml`), REST API (`/api/v1/*`,
  `CrudApiController<TDto>`, `[ApiKeyAuthorize]` ile `X-Api-Key`).

## Yeni içerik tipi eklerken

1. Domain'e entity (`IOrderable`, `IPublishable`), Application'a DTO (`IHasId`) + mapping + `I...Service : ICrudService<TDto>` + servis.
2. `Application/DependencyInjection.cs` ve `Infrastructure/DependencyInjection.cs` kayıtları, `MockDataProvider`'a örnek veri.
3. Admin: `ContentControllers.cs`'e controller, `Areas/Admin/Views/<Ad>/Index.cshtml` + `Form.cshtml`, `_AdminLayout` menüsüne link.
4. API: `ContentApiControllers.cs`'e `[Route("api/v1/...")]` controller.
5. `tests/CvPortal.Tests` altına servis testi.

## Kurallar

- Tasarım token'ları `wwwroot/css/site.css` (public) ve `admin.css` (admin) içindeki CSS değişkenleridir; açık/koyu tema `data-bs-theme` ile.
- Admin POST formları `AdminControllerBase` üzerindeki `[AutoValidateAntiforgeryToken]` ile korunur; silme formlarına `data-confirm` ekle.
- `appsettings.json` içindeki `Admin` ve `Api` değerleri demo amaçlıdır; gerçek sırları commit'leme (User Secrets / ortam değişkeni).
