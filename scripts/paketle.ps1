# CvPortal satış paketlerini üretir:
#   artifacts\CvPortal-<sürüm>.zip       Standart  — .NET 10 runtime gerektirir (hosting firmaları için ideal, küçük)
#   artifacts\CvPortal-<sürüm>-tam.zip   Tam       — .NET dahil, kurulum gerektirmez (çift tıkla çalıştır)
#   artifacts\SHA256SUMS.txt             Zip dosyalarının SHA-256 özetleri
#
# Kullanım (proje kökünden):  powershell -ExecutionPolicy Bypass -File scripts\paketle.ps1
# Sürüm numarası src\CvPortal.Web\CvPortal.Web.csproj → <Version> değerinden okunur.
param([switch]$SkipTests)

$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
Set-Location $root
Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem

[xml]$csproj = Get-Content 'src\CvPortal.Web\CvPortal.Web.csproj'
$version = ($csproj.Project.PropertyGroup | Where-Object { $_.Version } | Select-Object -First 1).Version
if (-not $version) { throw 'CvPortal.Web.csproj içinde <Version> bulunamadı.' }
Write-Host "CvPortal $version paketleniyor..." -ForegroundColor Cyan

if (git status --porcelain) {
    Write-Warning 'Commit edilmemiş değişiklikler var; paket bu değişiklikleri de içerecek.'
}

if (-not $SkipTests) {
    dotnet test CvPortal.slnx -nologo -v q
    if ($LASTEXITCODE -ne 0) { throw 'Testler başarısız; paket oluşturulmadı.' }
}

$artifacts = Join-Path $root 'artifacts'
foreach ($old in 'CvPortal', 'CvPortal-tam', 'paket') {
    $path = Join-Path $artifacts $old
    if (Test-Path $path) { Remove-Item $path -Recurse -Force }
}

$variants = @(
    @{ Profile = 'SatisPaketi';    Publish = 'CvPortal';     Suffix = '';
       Title = 'Standart (.NET 10 runtime gerektirir)';
       Requirement = "  - ASP.NET Core 10 Runtime (IIS için `"Hosting Bundle`")`n      https://dotnet.microsoft.com/download/dotnet/10.0" },
    @{ Profile = 'SatisPaketiTam'; Publish = 'CvPortal-tam'; Suffix = '-tam';
       Title = 'Tam (.NET dahil, ayrıca kurulum gerekmez)';
       Requirement = "  - .NET kurulumu GEREKMEZ, uygulamanın içinde gelir.`n  - Yalnızca IIS'te yayınlayacaksanız: .NET 10 `"Hosting Bundle`"`n      https://dotnet.microsoft.com/download/dotnet/10.0" }
)

$template = [IO.File]::ReadAllText((Join-Path $PSScriptRoot 'paket\BENIOKU.txt'))
$sums = @()

foreach ($v in $variants) {
    $name = "CvPortal-$version$($v.Suffix)"
    Write-Host "`n[$name] yayınlanıyor..." -ForegroundColor Cyan
    dotnet publish src\CvPortal.Web "-p:PublishProfile=$($v.Profile)" -nologo -v q
    if ($LASTEXITCODE -ne 0) { throw "$($v.Profile) yayınlanamadı." }

    $pkg = Join-Path $artifacts "paket\$name"
    New-Item -ItemType Directory -Force (Join-Path $pkg 'uygulama'), (Join-Path $pkg 'database') | Out-Null
    Copy-Item (Join-Path $artifacts "$($v.Publish)\*") (Join-Path $pkg 'uygulama') -Recurse

    # Müşteriye gitmemesi gerekenler: hata ayıklama sembolleri ve çalışma verisi (oturum anahtarları, yüklemeler).
    Get-ChildItem (Join-Path $pkg 'uygulama') -Recurse -Filter *.pdb | Remove-Item
    $appData = Join-Path $pkg 'uygulama\App_Data'
    if (Test-Path $appData) { Remove-Item $appData -Recurse -Force }

    Copy-Item 'database\*.sql' (Join-Path $pkg 'database')
    Copy-Item 'KURULUM.md', 'docs\CvPortal-Kullanim-Kilavuzu.pdf' $pkg

    # BENİOKU: Not Defteri'nde Türkçe karakterler doğru görünsün diye UTF-8 (BOM) + CRLF.
    $readme = $template.Replace('{{SURUM}}', $version).Replace('{{PAKET_ADI}}', $v.Title).Replace('{{GEREKSINIM}}', $v.Requirement)
    $readme = $readme.Replace("`r`n", "`n").Replace("`n", "`r`n")
    [IO.File]::WriteAllText((Join-Path $pkg 'BENIOKU.txt'), $readme, (New-Object Text.UTF8Encoding $true))

    # Zip: klasör ayracı "/" (Mac / Linux / hosting panellerinde de doğru açılır).
    $zip = Join-Path $artifacts "$name.zip"
    if (Test-Path $zip) { Remove-Item $zip }
    $stream = [IO.File]::Open($zip, 'CreateNew')
    $archive = New-Object IO.Compression.ZipArchive($stream, [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($file in Get-ChildItem $pkg -Recurse -File) {
            $entry = "$name/" + $file.FullName.Substring($pkg.Length + 1).Replace('\', '/')
            [void][IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, $entry, [IO.Compression.CompressionLevel]::Optimal)
        }
    }
    finally {
        $archive.Dispose(); $stream.Dispose()
    }

    $hash = (Get-FileHash $zip -Algorithm SHA256).Hash.ToLowerInvariant()
    $sums += "$hash  $name.zip"
    Write-Host ("  {0}.zip  {1:N1} MB  ({2} dosya)" -f $name, ((Get-Item $zip).Length / 1MB), (Get-ChildItem $pkg -Recurse -File).Count) -ForegroundColor Green
}

[IO.File]::WriteAllLines((Join-Path $artifacts 'SHA256SUMS.txt'), $sums)
Write-Host "`nSHA-256:" -ForegroundColor Cyan
$sums | ForEach-Object { "  $_" }
