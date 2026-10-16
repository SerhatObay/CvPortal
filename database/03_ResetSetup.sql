/* =====================================================================
   CvPortal · 03 - Kurulumu sıfırla (destek amaçlı)
   ---------------------------------------------------------------------
   Tüm yönetici hesaplarını siler ve kurulum işaretini kaldırır.
   Siteyi bir sonraki açışınızda /setup sihirbazı tekrar görünür.
   CV içeriğiniz (profil, deneyim, projeler vb.) SİLİNMEZ.

   Yalnızca şifresini unutup "reset-password" komutunu da
   çalıştıramayan müşteriler için kullanın.
   Uygulamayı yeniden başlatmanız gerekir (IIS: uygulama havuzunu geri dönüştürün).
   ===================================================================== */

USE [CvPortal];
GO

BEGIN TRANSACTION;

DELETE FROM [AdminUsers];
UPDATE [SiteSettings] SET [SetupCompletedAt] = NULL;

COMMIT TRANSACTION;

PRINT N'Kurulum sıfırlandı. Uygulamayı yeniden başlatıp /setup adresini açın.';
GO
