# Скрипт для проверки установки .NET SDK

Write-Host "Проверка установки .NET SDK..." -ForegroundColor Cyan
Write-Host ""

# Проверка команды dotnet
try {
    $dotnetVersion = dotnet --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ .NET SDK установлен: $dotnetVersion" -ForegroundColor Green
        
        # Проверка версии
        $versionParts = $dotnetVersion.Split('.')
        $majorVersion = [int]$versionParts[0]
        
        if ($majorVersion -ge 8) {
            Write-Host "✓ Версия .NET SDK подходит (требуется 8.0 или выше)" -ForegroundColor Green
        } else {
            Write-Host "✗ Версия .NET SDK слишком старая (установлена: $dotnetVersion, требуется: 8.0 или выше)" -ForegroundColor Yellow
            Write-Host "  Скачайте .NET 8.0 SDK с https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
        }
        
        # Список установленных SDK
        Write-Host ""
        Write-Host "Установленные SDK:" -ForegroundColor Cyan
        dotnet --list-sdks
        
    } else {
        throw "Команда dotnet не выполнена"
    }
} catch {
    Write-Host "✗ .NET SDK не установлен или не найден в PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "Установка .NET 8.0 SDK:" -ForegroundColor Yellow
    Write-Host "1. Перейдите на https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Write-Host "2. Скачайте .NET 8.0 SDK для Windows" -ForegroundColor Yellow
    Write-Host "3. Запустите установщик" -ForegroundColor Yellow
    Write-Host "4. Перезапустите PowerShell после установки" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Или используйте winget:" -ForegroundColor Yellow
    Write-Host "  winget install Microsoft.DotNet.SDK.8" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Проверка завершена." -ForegroundColor Cyan
Write-Host ""
Write-Host "Для подробной инструкции см. файл INSTALLATION.md" -ForegroundColor Cyan

