namespace CvPortal.Infrastructure;

public enum DatabaseProvider
{
    /// <summary>Microsoft SQL Server (Express, LocalDB, Standard, Azure SQL).</summary>
    SqlServer,

    /// <summary>Veritabanı olmadan, bellek içi demo modu. Veriler uygulama kapanınca kaybolur.</summary>
    InMemory
}

/// <summary>
/// appsettings.json → "Database" bölümü ve "ConnectionStrings:DefaultConnection".
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;

    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Uygulama açılırken bekleyen migration'ları otomatik uygular (veritabanını yoksa oluşturur).</summary>
    public bool AutoMigrate { get; set; } = true;

    /// <summary>Veritabanı boşsa örnek CV içeriğini ekler. Kurulum sihirbazında temizlenebilir.</summary>
    public bool SeedDemoData { get; set; } = true;
}
