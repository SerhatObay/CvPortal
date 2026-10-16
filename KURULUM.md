# CvPortal — Kurulum Kılavuzu

Kişisel CV / portfolyo sitesi ve yönetim paneli. Bu kılavuz, uygulamayı kendi bilgisayarınızda veya bir Windows sunucuda (IIS) çalıştırmak için gereken her şeyi anlatır.

> **Kısaca:** Bağlantı cümlesini girin → uygulamayı başlatın → tarayıcıda `/setup` sihirbazını tamamlayın. Veritabanı ve tablolar otomatik oluşturulur.

---

## 1. Gereksinimler

| Bileşen | Sürüm | Not |
|---|---|---|
| İşletim sistemi | Windows 10/11 veya Windows Server 2016+ | |
| .NET | **ASP.NET Core 10 Runtime** (IIS için **Hosting Bundle**) | [dotnet.microsoft.com/download/dotnet/10.0](https://dotnet.microsoft.com/download/dotnet/10.0) |
| Veritabanı | **SQL Server 2016 veya üzeri** | Ücretsiz **SQL Server Express** yeterlidir |
| Yönetim aracı (isteğe bağlı) | SQL Server Management Studio (SSMS) | Yedek alma, elle kurulum için |

---

## 2. Hızlı başlangıç (kendi bilgisayarınızda)

1. Paketi bir klasöre çıkarın, ör. `C:\CvPortal`.
2. `appsettings.json` dosyasını Not Defteri ile açın ve `DefaultConnection` değerini kontrol edin.
   Bilgisayarınızda **SQL Server Express** kuruluysa varsayılan değer olduğu gibi çalışır:
   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CvPortal;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
   ```
3. `CvPortal.Web.exe` dosyasına çift tıklayın. Konsolda `Now listening on: http://localhost:5000` yazısını görün.
4. Tarayıcıda **http://localhost:5000/setup** adresini açın ve kurulum sihirbazını tamamlayın.

İlk açılışta uygulama `CvPortal` veritabanını, tüm tabloları ve örnek CV içeriğini **kendisi oluşturur**.

---

## 3. Kurulum sihirbazı (`/setup`)

Sihirbaz yalnızca ilk yönetici hesabı oluşturulana kadar görünür.

| Alan | Açıklama |
|---|---|
| Ad Soyad, Ünvan, E-posta | Sitenizin üst kısmında ve iletişim bölümünde görünür |
| Kullanıcı adı / Şifre | Admin paneline giriş bilgileriniz. Şifre en az 8 karakter, harf + rakam içermeli |
| Örnek içeriği sil | Açarsanız boş bir CV ile başlarsınız. Kapalı bırakırsanız örnek içeriği kendi bilgilerinizle değiştirirsiniz |
| REST API | Harici bir panel / mobil uygulama kullanmayacaksanız kapalı bırakın |

Kurulumdan sonra admin paneli: **/Admin**

---

## 4. Sunucuya kurulum (IIS)

1. Sunucuya **.NET 10 Hosting Bundle**'ı kurun ve sunucuyu yeniden başlatın (ya da `iisreset`).
2. Paketi sunucuya kopyalayın, ör. `C:\inetpub\CvPortal`.
3. **IIS Manager** → *Application Pools* → *Add Application Pool*:
   - Ad: `CvPortal`
   - .NET CLR version: **No Managed Code**
   - Gelişmiş ayarlar → *Load User Profile*: **True**
4. *Sites* → *Add Website*: fiziksel yol `C:\inetpub\CvPortal`, uygulama havuzu `CvPortal`, alan adınız ve bağlantı noktası.
5. Klasör izni: `C:\inetpub\CvPortal\App_Data` klasörünü oluşturun ve `IIS AppPool\CvPortal` kullanıcısına **Değiştirme (Modify)** izni verin. Oturum anahtarları burada saklanır.
6. Veritabanı izni — iki seçenekten biri:
   - **Windows kimliği:** SSMS'te `database/01_CreateDatabase.sql` dosyasındaki *SEÇENEK A* satırlarını açıp çalıştırın. Bağlantı cümlesi `Trusted_Connection=True` kalır.
   - **SQL kullanıcı adı/şifre** (hosting firmaları genelde bunu verir):
     ```json
     "DefaultConnection": "Server=SUNUCU;Database=CvPortal;User Id=KULLANICI;Password=SIFRE;TrustServerCertificate=True;MultipleActiveResultSets=True"
     ```
7. Siteyi tarayıcıda açın → `/setup`.

> **HTTPS:** Canlı sitede IIS'e bir SSL sertifikası bağlayın (ör. win-acme ile ücretsiz Let's Encrypt). Uygulama HTTP isteklerini otomatik olarak HTTPS'e yönlendirir.

---

## 5. Veritabanını SSMS ile elle hazırlamak (isteğe bağlı)

Uygulama kullanıcısının veritabanı oluşturma yetkisi yoksa ya da şemayı kendiniz yönetmek istiyorsanız:

1. SSMS'te `database/01_CreateDatabase.sql` → **Execute (F5)**
2. `database/02_CreateSchema.sql` → **Execute (F5)** (tekrar çalıştırmak güvenlidir)
3. `appsettings.json` → `"AutoMigrate": false`

| Script | Ne işe yarar |
|---|---|
| `01_CreateDatabase.sql` | `CvPortal` veritabanını oluşturur, isteğe bağlı uygulama kullanıcısı |
| `02_CreateSchema.sql` | Tüm tabloları oluşturur / günceller (idempotent) |
| `03_ResetSetup.sql` | Yönetici hesaplarını silip kurulum sihirbazını yeniden açar (içerik silinmez) |
| `04_Backup.sql` | Tarih damgalı tam yedek (.bak) alır |

---

## 6. Şifremi unuttum

Sunucuda uygulama klasöründe bir komut istemi (cmd) açın:

```bat
cd C:\inetpub\CvPortal
CvPortal.Web.exe reset-password admin YeniSifre2026
```

Kullanıcı adınızı hatırlamıyorsanız yanlış bir ad yazın; komut mevcut kullanıcıları listeler.
Komutu çalıştıramıyorsanız SSMS'te `database/03_ResetSetup.sql` çalıştırın, uygulama havuzunu geri dönüştürün ve `/setup` ile yeni hesap oluşturun.

---

## 7. Yedekleme

- **SSMS:** `database/04_Backup.sql` → F5. Dosya SQL Server'ın *Backup* klasörüne yazılır.
- **Geri yükleme:** SSMS → *Databases* → sağ tık → *Restore Database…*
- Düzenli yedek için Windows Görev Zamanlayıcı + `sqlcmd -S .\SQLEXPRESS -i 04_Backup.sql` kullanılabilir.

---

## 8. Güncelleme

1. Yedek alın.
2. Siteyi durdurun (IIS: uygulama havuzunu durdurun).
3. Yeni paketteki dosyaları kopyalayın — **`appsettings.json` ve `App_Data` klasörünün üzerine yazmayın.**
4. Siteyi başlatın. Yeni veritabanı değişiklikleri açılışta otomatik uygulanır.

---

## 9. Ayarlar (`appsettings.json`)

| Ayar | Varsayılan | Açıklama |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | `.\SQLEXPRESS` | SQL Server bağlantısı |
| `Database:Provider` | `SqlServer` | `InMemory` → veritabanısız demo modu (veriler kapanınca silinir) |
| `Database:AutoMigrate` | `true` | Açılışta tabloları oluşturur / günceller |
| `Database:SeedDemoData` | `true` | Boş veritabanına örnek CV içeriği ekler |
| `Api:AllowedOrigins` | `[]` | REST API'yi tarayıcıdan çağıracak harici panel adresleri |

REST API anahtarı, yönetici şifresi ve hesap bilgileri dosyada **değil**, veritabanında saklanır ve admin panel → **Ayarlar** ekranından yönetilir.

---

## 10. Sorun giderme

| Belirti | Çözüm |
|---|---|
| Uygulama açılmıyor, konsolda *"Veritabanına bağlanılamadı"* | SQL Server servisi çalışıyor mu? (`services.msc` → *SQL Server (SQLEXPRESS)*). Bağlantı cümlesindeki sunucu adını kontrol edin |
| IIS: **HTTP Error 500.19** | .NET Hosting Bundle kurulu değil |
| IIS: **HTTP Error 500.30** | Uygulama başlarken hata aldı — genellikle veritabanı bağlantısı / izni. *Olay Görüntüleyicisi → Windows Günlükleri → Uygulama* kayıtlarına bakın |
| *Login failed for user 'IIS APPPOOL\…'* | 4. bölüm, 6. adım: uygulama havuzuna veritabanı izni verin |
| Her yeniden başlatmada oturum kapanıyor | `App_Data` klasörüne yazma izni ve uygulama havuzunda *Load User Profile = True* |
| Sağlık kontrolü | `https://siteniz/health` → `Healthy` yazmalı |
