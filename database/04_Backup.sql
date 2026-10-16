/* =====================================================================
   CvPortal · 04 - Yedek alma
   ---------------------------------------------------------------------
   Tam yedek (.bak) alır. Dosya, SQL Server'ın varsayılan yedek klasörüne
   tarih-saat damgalı olarak yazılır (ör. ...\MSSQL\Backup\CvPortal_20261016_1430.bak).
   Farklı bir klasör istiyorsanız @folder değerini değiştirin; klasöre
   SQL Server servis hesabının yazma izni olmalıdır.

   Geri yükleme: SSMS → Databases → sağ tık → Restore Database...
   ===================================================================== */

DECLARE @folder nvarchar(300) = CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS nvarchar(300));
DECLARE @path nvarchar(400) = @folder + N'\CvPortal_' + FORMAT(SYSDATETIME(), 'yyyyMMdd_HHmm') + N'.bak';

-- Not: SQL Server Express yedek sıkıştırmayı desteklemez; Standard/Enterprise'da "COMPRESSION," ekleyebilirsiniz.
BACKUP DATABASE [CvPortal]
TO DISK = @path
WITH INIT, CHECKSUM, NAME = N'CvPortal tam yedek';

PRINT N'Yedek alındı: ' + @path;
GO
