# Construit le zip portable (sans admin) et tente un MSIX non signe.
$ErrorActionPreference = "Stop"
$racine = Split-Path -Parent $PSScriptRoot
$sortie = Join-Path $racine "artifacts\portable"
$zip = Join-Path $racine "artifacts\CursorFrancais-portable.zip"
$msixDir = Join-Path $racine "artifacts\msix"

New-Item -ItemType Directory -Force -Path $sortie | Out-Null
dotnet publish (Join-Path $racine "src\CursorFrancais.App\CursorFrancais.App.csproj") `
    -c Release -r win-x64 --self-contained false -o $sortie
Copy-Item (Join-Path $racine "LICENSE") $sortie -Force
Copy-Item (Join-Path $racine "NOTICE") $sortie -Force
Copy-Item (Join-Path $racine "docs\disclaimer-fr.md") $sortie -Force

if (Test-Path $zip) { Remove-Item $zip }
Compress-Archive -Path (Join-Path $sortie "*") -DestinationPath $zip
Write-Host "Zip : $zip"

$makeappx = Get-Command makeappx -ErrorAction SilentlyContinue
if ($makeappx) {
    New-Item -ItemType Directory -Force -Path $msixDir | Out-Null
    Copy-Item (Join-Path $PSScriptRoot "AppxManifest.xml") $msixDir -Force
    Copy-Item (Join-Path $sortie "*") $msixDir -Force
    $msix = Join-Path $racine "artifacts\CursorFrancais-unsigned.msix"
    & makeappx pack /d $msixDir /p $msix /o
    Write-Host "MSIX non signe : $msix"
} else {
    Write-Host "makeappx absent : zip seulement (MSIX unsigned reporte)."
}
