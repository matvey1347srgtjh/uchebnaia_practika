# Скрипт для установки клиентских библиотек
# Используйте этот скрипт, если libman restore не работает

Write-Host "Установка клиентских библиотек..." -ForegroundColor Cyan
Write-Host ""

# Создание структуры папок
Write-Host "Создание структуры папок..." -ForegroundColor Yellow
$folders = @(
    "wwwroot/lib/jquery/dist",
    "wwwroot/lib/bootstrap/dist/css",
    "wwwroot/lib/bootstrap/dist/js",
    "wwwroot/lib/jquery-validation/dist",
    "wwwroot/lib/jquery-validation-unobtrusive"
)

foreach ($folder in $folders) {
    if (-not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "  ✓ Создана папка: $folder" -ForegroundColor Green
    } else {
        Write-Host "  - Папка уже существует: $folder" -ForegroundColor Gray
    }
}

Write-Host ""

# Функция для скачивания файлов
function Download-File {
    param (
        [string]$Url,
        [string]$OutputPath
    )
    
    try {
        $folder = Split-Path -Parent $OutputPath
        if (-not (Test-Path $folder)) {
            New-Item -ItemType Directory -Path $folder -Force | Out-Null
        }
        
        Write-Host "  Скачивание: $Url" -ForegroundColor Yellow
        Invoke-WebRequest -Uri $Url -OutFile $OutputPath -UseBasicParsing -ErrorAction Stop
        Write-Host "  ✓ Установлен: $OutputPath" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "  ✗ Ошибка при скачивании: $_" -ForegroundColor Red
        return $false
    }
}

# Скачивание библиотек
Write-Host "Скачивание библиотек..." -ForegroundColor Yellow
Write-Host ""

$success = $true

# jQuery
$success = (Download-File -Url "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js" -OutputPath "wwwroot/lib/jquery/dist/jquery.min.js") -and $success

# Bootstrap CSS
$success = (Download-File -Url "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/css/bootstrap.min.css" -OutputPath "wwwroot/lib/bootstrap/dist/css/bootstrap.min.css") -and $success

# Bootstrap JS
$success = (Download-File -Url "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/js/bootstrap.bundle.min.js" -OutputPath "wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js") -and $success

# jQuery Validation
$success = (Download-File -Url "https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.5/jquery.validate.min.js" -OutputPath "wwwroot/lib/jquery-validation/dist/jquery.validate.min.js") -and $success

# jQuery Validation Unobtrusive
$success = (Download-File -Url "https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js" -OutputPath "wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js") -and $success

Write-Host ""

# Проверка установки
Write-Host "Проверка установленных файлов..." -ForegroundColor Cyan
$files = @(
    "wwwroot/lib/jquery/dist/jquery.min.js",
    "wwwroot/lib/bootstrap/dist/css/bootstrap.min.css",
    "wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js",
    "wwwroot/lib/jquery-validation/dist/jquery.validate.min.js",
    "wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"
)

$allInstalled = $true
foreach ($file in $files) {
    if (Test-Path $file) {
        $size = (Get-Item $file).Length
        Write-Host "  ✓ $file ($([math]::Round($size/1KB, 2)) KB)" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file - НЕ НАЙДЕН" -ForegroundColor Red
        $allInstalled = $false
    }
}

Write-Host ""

if ($allInstalled -and $success) {
    Write-Host "✓ Все библиотеки успешно установлены!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Теперь вы можете запустить проект:" -ForegroundColor Cyan
    Write-Host "  dotnet restore" -ForegroundColor Yellow
    Write-Host "  dotnet build" -ForegroundColor Yellow
    Write-Host "  dotnet run" -ForegroundColor Yellow
} else {
    Write-Host "✗ При установке возникли ошибки." -ForegroundColor Red
    Write-Host "Попробуйте установить библиотеки вручную. См. INSTALL_LIBRARIES.md" -ForegroundColor Yellow
}

