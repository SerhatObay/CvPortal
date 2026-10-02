# CvPortal

Yazılım mühendisleri için **admin panel** ve **REST API** ile yönetilebilen kişisel CV / portfolyo uygulaması.
.NET 10 · ASP.NET Core MVC · Clean Architecture · Mock (in-memory) veri.

## Çalıştırma

**Visual Studio 2026:** `CvPortal.slnx` dosyasını açın → başlangıç projesi `CvPortal.Web` → **F5**.

**Komut satırı:**

```bash
dotnet run --project src/CvPortal.Web
```

| Adres | Açıklama |
|---|---|
| `/` | Herkese açık CV sayfası |
| `/Admin` | Yönetim paneli (giriş bilgileri: `appsettings.json → Admin`) |
| `/scalar/v1` | API dokümantasyonu (Scalar) |
| `/openapi/v1.json` | OpenAPI şeması |

> Veriler bellekte tutulur; admin panelden yapılan değişiklikler uygulama yeniden başlatılınca mock verilere döner.

## Proje yapısı

```
CvPortal.slnx
├─ src/
│  ├─ CvPortal.Domain          Entity'ler, enum'lar (bağımlılıksız)
│  ├─ CvPortal.Application     DTO'lar, doğrulama, servis arayüzleri ve iş kuralları
│  ├─ CvPortal.Infrastructure  InMemoryRepository + MockDataProvider (örnek CV verisi)
│  └─ CvPortal.Web             MVC site, Admin Area, REST API, cookie auth
└─ tests/
   └─ CvPortal.Tests           xUnit birim testleri
```

### Servisler

| Servis | Görev |
|---|---|
| `IProfileService` | Tekil profil bilgisi (ad, ünvan, iletişim, sosyal linkler) |
| `IExperienceService` | İş deneyimleri (CRUD, görünürlük, sıralama) |
| `IEducationService` | Eğitim bilgileri |
| `ISkillService` | Yetenekler + kategoriye göre gruplama |
| `IProjectService` | Projeler + "öne çıkan" yönetimi |
| `ICertificateService` | Sertifikalar |
| `ILanguageService` | Yabancı diller |
| `IContactMessageService` | İletişim formu mesajları (okundu/okunmadı) |
| `ICvService` | Yayındaki tüm içeriği tek modelde toplar (public sayfa + API) |
| `IDashboardService` | Admin özet istatistikleri, profil doluluk oranı |

İçerik servisleri ortak `CrudServiceBase<TEntity, TDto>` sınıfından türer; admin ve API controller'ları da
`CrudAdminController<TDto>` / `CrudApiController<TDto>` ile ortak davranışı paylaşır.

## Admin panel

Dashboard · Profil · Deneyimler · Eğitim · Yetenekler · Projeler · Sertifikalar · Diller · Mesajlar

- Ekle / düzenle / sil (onay penceresiyle), sitede gizle/göster, sıralama
- Sunucu + istemci tarafı doğrulama (Türkçe mesajlar)
- Açık/koyu tema, mobil uyumlu kenar menü, tablo arama ve filtreleme

## Başka bir panelden yönetim (REST API)

Yönetim uç noktaları `X-Api-Key` başlığı ister (`appsettings.json → Api:ApiKey`).
React/Vue gibi bir panelden tarayıcı ile çağrılacaksa adresini `Api:AllowedOrigins` listesine ekleyin (CORS).

```bash
curl -H "X-Api-Key: <anahtar>" http://localhost:5081/api/v1/experiences
```

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

## Gerçek veritabanına geçiş

Uygulama yalnızca `IRepository<T>` arayüzüne bağlıdır. EF Core'a geçmek için:

1. `CvPortal.Infrastructure` içine bir `DbContext` ve `EfRepository<T> : IRepository<T>` ekleyin.
2. `Infrastructure/DependencyInjection.cs` içindeki `InMemoryRepository` kayıtlarını `EfRepository` ile değiştirin.
3. Mock verileri seed olarak kullanmak için `MockDataProvider` metotlarından yararlanın.

## Canlıya almadan önce

- `Admin:Password` ve `Api:ApiKey` değerlerini User Secrets / ortam değişkenleri ile değiştirin
  (`Admin__Password`, `Api__ApiKey`). Şifreyi hash'lenmiş olarak saklamak daha güvenlidir.
- Profil fotoğrafı ve CV PDF'ini `wwwroot/images`, `wwwroot/files` altına koyup admin panelden yolunu girin.

## Testler

```bash
dotnet test
```
