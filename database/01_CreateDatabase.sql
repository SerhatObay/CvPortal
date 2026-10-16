/* =====================================================================
   CvPortal · 01 - Veritabanı oluşturma
   ---------------------------------------------------------------------
   SSMS'te "master" veritabanına bağlıyken çalıştırın (F5).
   Veritabanı zaten varsa hiçbir şey yapmaz.

   NOT: Uygulama varsayılan ayarlarla (Database:AutoMigrate = true)
   veritabanını ilk açılışta kendisi oluşturur. Bu scripti yalnızca
   veritabanını önceden / elle oluşturmak istiyorsanız kullanın
   (ör. uygulama kullanıcısının CREATE DATABASE yetkisi yoksa).
   ===================================================================== */

USE [master];
GO

IF DB_ID(N'CvPortal') IS NULL
BEGIN
    -- Sunucunun varsayılan collation'ı kullanılır (uygulamanın otomatik oluşturduğu veritabanıyla aynı).
    CREATE DATABASE [CvPortal];
    PRINT N'CvPortal veritabanı oluşturuldu.';
END
ELSE
BEGIN
    PRINT N'CvPortal veritabanı zaten var, atlandı.';
END
GO

-- ---------------------------------------------------------------------
--  (İsteğe bağlı) Uygulamanın bağlanacağı SQL Server kullanıcısı
-- ---------------------------------------------------------------------
--  Kendi bilgisayarınızda Windows kimlik doğrulamasıyla çalışıyorsanız bu
--  bölüme gerek yoktur. Sunucuya (IIS) kurulumda aşağıdaki iki seçenekten
--  BİRİNİN satır başlarındaki "--" işaretlerini kaldırıp çalıştırın.
--
--  Uygulamanın tabloları kendisi oluşturabilmesi için db_owner rolü verilir.
--  İsterseniz kurulumdan sonra db_datareader + db_datawriter rollerine
--  düşürebilirsiniz (bu durumda appsettings.json → Database:AutoMigrate = false yapın).
--
--  SEÇENEK A — IIS uygulama havuzu kimliği (Windows kimlik doğrulaması).
--  "CvPortal" yerine IIS'teki uygulama havuzunuzun adını yazın.
--
-- CREATE LOGIN [IIS APPPOOL\CvPortal] FROM WINDOWS;
-- GO
-- USE [CvPortal];
-- CREATE USER [IIS APPPOOL\CvPortal] FOR LOGIN [IIS APPPOOL\CvPortal];
-- ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\CvPortal];
-- GO
--
--  SEÇENEK B — SQL Server kullanıcı adı / şifresi.
--  Şifreyi mutlaka değiştirin; aynı bilgileri bağlantı cümlesine yazın:
--  "Server=...;Database=CvPortal;User Id=cvportal_app;Password=...;TrustServerCertificate=True"
--  (SQL Server'da "SQL Server and Windows Authentication mode" açık olmalıdır.)
--
-- CREATE LOGIN [cvportal_app] WITH PASSWORD = N'BURAYA-GUCLU-BIR-SIFRE', CHECK_POLICY = ON;
-- GO
-- USE [CvPortal];
-- CREATE USER [cvportal_app] FOR LOGIN [cvportal_app];
-- ALTER ROLE db_owner ADD MEMBER [cvportal_app];
-- GO
