@echo off
echo ========================================
echo  Home Assistant Plugin - Release Build
echo ========================================
echo.
cd /d "%~dp0"
dotnet build src\HomeAssistantByBatuPlugin.csproj -c Release
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo BUILD FAILED!
    pause
    exit /b 1
)
echo.
echo Done! The .lplug4 file is in the output\ folder.
echo Right-click it and select "Install Plugin" to install.
echo.
pause
