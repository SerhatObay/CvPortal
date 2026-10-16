# CvPortal kullanım kılavuzunu kilavuz.html'den PDF'e çevirir (Microsoft Edge, görünmez modda).
# Kullanım (proje kökünden):  powershell -ExecutionPolicy Bypass -File docs\kilavuz-kaynak\build.ps1
# Ekran görüntülerini güncellemek için img\ klasöründeki aynı adlı dosyaları değiştirmeniz yeterlidir.
param([string]$Output = (Join-Path $PSScriptRoot '..\CvPortal-Kullanim-Kilavuzu.pdf'))

$Output = [IO.Path]::GetFullPath($Output)
$html = Join-Path $PSScriptRoot 'kilavuz.html'
$url = ([Uri]$html).AbsoluteUri
$profileDir = Join-Path $PSScriptRoot '.edge-profile'   # Edge'in geçici profili (git'e girmez)

$edge = @(
    "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe",
    "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $edge) { "Microsoft Edge bulunamadı."; exit 1 }

if (Test-Path $Output) { Remove-Item -LiteralPath $Output }

$p = Start-Process $edge -PassThru -Wait -WindowStyle Hidden -ArgumentList @(
    '--headless=new', '--disable-gpu', "--user-data-dir=`"$profileDir`"",
    '--no-pdf-header-footer', '--run-all-compositor-stages-before-draw',
    '--virtual-time-budget=8000', "--print-to-pdf=`"$Output`"", $url)

if (-not (Test-Path $Output)) { "PDF oluşmadı (çıkış kodu $($p.ExitCode))"; exit 1 }

$text = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($Output))
"PDF: {0} ({1} KB, {2} sayfa)" -f $Output, [int]((Get-Item $Output).Length / 1KB), ([regex]::Matches($text, '/Type\s*/Page[^s]')).Count
