param(
    [string]$RimWorldDir = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld",
    [string]$ModFolderName = "ChildrenOfArrakis",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

$managedDir = Join-Path $RimWorldDir "RimWorldWin64_Data\Managed"
$csproj = Join-Path $root "Source\ChildrenOfArrakis.csproj"

Write-Host "Building $csproj -> Assemblies (config: $Configuration)"
dotnet build $csproj -c $Configuration /p:RimWorldManagedDir="$managedDir"

$modsDir = Join-Path $RimWorldDir "Mods"
if (-not (Test-Path $modsDir)) {
    New-Item -ItemType Directory -Force -Path $modsDir | Out-Null
}

$dest = Join-Path $modsDir $ModFolderName
Write-Host "Removing existing mod folder at $dest (if any)"
if (Test-Path $dest) {
    Remove-Item -Recurse -Force $dest
}
New-Item -ItemType Directory -Force -Path $dest | Out-Null

$payload = @("About", "Assemblies", "Defs", "Patches", "Textures", "Languages", "LICENSE", "README.md")
foreach ($item in $payload) {
    $src = Join-Path $root $item
    if (Test-Path $src) {
        Write-Host "Copying $src -> $dest"
        Copy-Item -Path $src -Destination $dest -Recurse -Force
    }
}

Write-Host "Deploy complete: $dest"
