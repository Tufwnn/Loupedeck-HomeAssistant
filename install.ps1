$pluginsDir = "$env:LOCALAPPDATA\Logi\LogiPluginService\Plugins"
$pluginName = "HomeAssistantByBatu"
$lplug4 = Join-Path $PSScriptRoot "output\HomeAssistantByBatuPlugin.lplug4"

if (-not (Test-Path $lplug4)) {
    Write-Host "ERROR: $lplug4 not found. Run 'dotnet build src\HomeAssistantByBatuPlugin.csproj -c Release' first." -ForegroundColor Red
    pause
    exit 1
}

Write-Host "Installing Home Assistant by Batu plugin..." -ForegroundColor Cyan

# Remove old versions
$oldNames = @("HomeAssistant", "HomeAssistantByBatu")
foreach ($name in $oldNames) {
    $path = Join-Path $pluginsDir $name
    if (Test-Path $path) {
        Write-Host "  Removing old: $name" -ForegroundColor Yellow
        Remove-Item -Recurse -Force $path
    }
}
$linkFile = Join-Path $pluginsDir "HomeAssistantByBatuPlugin.link"
if (Test-Path $linkFile) {
    Remove-Item -Force $linkFile
}

# Copy as .zip for Expand-Archive compatibility
$tempZip = Join-Path $env:TEMP "HomeAssistantByBatuPlugin.zip"
Copy-Item $lplug4 $tempZip -Force

# Extract new plugin
$destDir = Join-Path $pluginsDir $pluginName
New-Item -ItemType Directory -Force -Path $destDir | Out-Null
Write-Host "  Extracting to: $destDir" -ForegroundColor Green
Expand-Archive -Path $tempZip -DestinationPath $destDir -Force
Remove-Item $tempZip -Force

Write-Host ""
Write-Host "Installed! Contents:" -ForegroundColor Green
Get-ChildItem -Recurse $destDir | ForEach-Object {
    Write-Host "  $($_.FullName.Replace($destDir, '.'))"
}

Write-Host ""
Write-Host "Now restart Loupedeck software or Logi Plugin Service." -ForegroundColor Cyan
