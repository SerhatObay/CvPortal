/* =====================================================================
   CvPortal · 02 - Tablolar (şema)
   ---------------------------------------------------------------------
   Bu script EF Core migration'larından otomatik üretilmiştir ve
   tekrar çalıştırılabilir (idempotent): yalnızca eksik olan adımlar uygulanır.

   NOT: Uygulama varsayılan olarak (Database:AutoMigrate = true) bu adımı
   açılışta kendisi yapar. Bu dosyayı yalnızca veritabanını SSMS ile elle
   hazırlamak istiyorsanız kullanın. Önce 01_CreateDatabase.sql çalıştırın.

   Yeniden üretmek için (proje kökünde):
   dotnet ef migrations script --idempotent --project src/CvPortal.Infrastructure --startup-project src/CvPortal.Web --output database/02_CreateSchema.sql
   ===================================================================== */

USE [CvPortal];
GO
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [AdminUsers] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [DisplayName] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [LastLoginAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_AdminUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Certificates] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(150) NOT NULL,
        [Issuer] nvarchar(100) NOT NULL,
        [IssueDate] date NOT NULL,
        [ExpirationDate] date NULL,
        [CredentialId] nvarchar(100) NULL,
        [CredentialUrl] nvarchar(300) NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [ContactMessages] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(80) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Subject] nvarchar(120) NOT NULL,
        [Message] nvarchar(3000) NOT NULL,
        [IsRead] bit NOT NULL,
        [ReadAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ContactMessages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Educations] (
        [Id] int NOT NULL IDENTITY,
        [School] nvarchar(150) NOT NULL,
        [Degree] nvarchar(100) NOT NULL,
        [FieldOfStudy] nvarchar(150) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NULL,
        [Grade] nvarchar(20) NULL,
        [Description] nvarchar(1000) NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Educations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Experiences] (
        [Id] int NOT NULL IDENTITY,
        [Company] nvarchar(100) NOT NULL,
        [CompanyUrl] nvarchar(300) NULL,
        [Position] nvarchar(100) NOT NULL,
        [Location] nvarchar(100) NOT NULL,
        [EmploymentType] nvarchar(30) NOT NULL,
        [WorkMode] nvarchar(30) NOT NULL,
        [StartDate] date NOT NULL,
        [EndDate] date NULL,
        [Description] nvarchar(2000) NOT NULL,
        [Achievements] nvarchar(max) NOT NULL,
        [Technologies] nvarchar(max) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Experiences] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Languages] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Level] nvarchar(30) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Languages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Profiles] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(100) NOT NULL,
        [Title] nvarchar(100) NOT NULL,
        [Headline] nvarchar(250) NOT NULL,
        [About] nvarchar(4000) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Phone] nvarchar(30) NULL,
        [Location] nvarchar(100) NOT NULL,
        [AvatarUrl] nvarchar(500) NULL,
        [CvFileUrl] nvarchar(500) NULL,
        [GitHubUrl] nvarchar(300) NULL,
        [LinkedInUrl] nvarchar(300) NULL,
        [WebsiteUrl] nvarchar(300) NULL,
        [MediumUrl] nvarchar(300) NULL,
        [CareerStartDate] date NOT NULL,
        [IsOpenToWork] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Profiles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Projects] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(100) NOT NULL,
        [Summary] nvarchar(300) NOT NULL,
        [Description] nvarchar(3000) NULL,
        [ImageUrl] nvarchar(500) NULL,
        [GitHubUrl] nvarchar(300) NULL,
        [LiveUrl] nvarchar(300) NULL,
        [Technologies] nvarchar(max) NOT NULL,
        [IsFeatured] bit NOT NULL,
        [CompletedAt] date NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [SiteSettings] (
        [Id] int NOT NULL IDENTITY,
        [ApiKey] nvarchar(100) NOT NULL,
        [IsApiEnabled] bit NOT NULL,
        [SetupCompletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SiteSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE TABLE [Skills] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(50) NOT NULL,
        [Category] nvarchar(30) NOT NULL,
        [Proficiency] int NOT NULL,
        [IconClass] nvarchar(80) NULL,
        [DisplayOrder] int NOT NULL,
        [IsVisible] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Skills] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Skills_Proficiency] CHECK ([Proficiency] BETWEEN 0 AND 100)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AdminUsers_Username] ON [AdminUsers] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Certificates_DisplayOrder] ON [Certificates] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_ContactMessages_IsRead_CreatedAt] ON [ContactMessages] ([IsRead], [CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Educations_DisplayOrder] ON [Educations] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Experiences_DisplayOrder] ON [Experiences] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Projects_DisplayOrder] ON [Projects] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Skills_Category_DisplayOrder] ON [Skills] ([Category], [DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261016053459_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261016053459_InitialCreate', N'10.0.13');
END;

COMMIT;
GO

