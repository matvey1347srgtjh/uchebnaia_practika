# Инструкция по установке

## Установка .NET SDK

### Шаг 1: Проверка наличия .NET SDK

Откройте PowerShell или командную строку и выполните:
```powershell
dotnet --version
```

Если команда не найдена, необходимо установить .NET SDK.

### Шаг 2: Установка .NET 8.0 SDK

#### Вариант 1: Скачать с официального сайта (рекомендуется)

1. Перейдите на сайт: https://dotnet.microsoft.com/download/dotnet/8.0
2. Скачайте **.NET 8.0 SDK** (не Runtime!) для Windows
3. Запустите установщик и следуйте инструкциям
4. После установки перезапустите PowerShell/командную строку
5. Проверьте установку: `dotnet --version` (должно показать версию 8.0.x)

#### Вариант 2: Через Chocolatey (если установлен)

```powershell
choco install dotnet-8.0-sdk
```

#### Вариант 3: Через winget (Windows 10/11)

```powershell
winget install Microsoft.DotNet.SDK.8
```

### Шаг 3: Проверка установки

После установки выполните:
```powershell
dotnet --version
dotnet --list-sdks
```

Должна отображаться версия 8.0.x.

## Установка клиентских библиотек

### Способ 1: Через LibMan (рекомендуется)

1. Установите LibMan CLI:
```powershell
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
```

2. Восстановите библиотеки:
```powershell
cd CinemaApp
libman restore
```

### Способ 2: Через Visual Studio

1. Откройте проект в Visual Studio
2. Правый клик на проекте → "Управление клиентскими библиотеками"
3. Нажмите "Восстановить клиентские библиотеки"

### Способ 3: Вручную (если LibMan не работает)

Создайте следующие папки и скачайте файлы:

```
wwwroot/lib/jquery/dist/jquery.min.js
wwwroot/lib/bootstrap/dist/css/bootstrap.min.css
wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js
wwwroot/lib/jquery-validation/dist/jquery.validate.min.js
wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js
```

**Ссылки для скачивания:**
- jQuery 3.7.1: https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js
- Bootstrap 5.3.0 CSS: https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/css/bootstrap.min.css
- Bootstrap 5.3.0 JS: https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/js/bootstrap.bundle.min.js
- jQuery Validation: https://cdnjs.cloudflare.com/ajax/libs/jquery-validation/1.19.5/jquery.validate.min.js
- jQuery Validation Unobtrusive: https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js

## Запуск проекта

После установки .NET SDK и клиентских библиотек:

```powershell
cd CinemaApp
dotnet restore
dotnet build
dotnet run
```

Откройте браузер: https://localhost:5001

## Устранение проблем

### Проблема: "dotnet не распознан как команда"

**Решение:**
1. Убедитесь, что .NET SDK установлен
2. Перезапустите PowerShell/командную строку
3. Проверьте переменную PATH: в PATH должна быть папка с dotnet.exe (обычно `C:\Program Files\dotnet\`)

### Проблема: "Не удается найти проект"

**Решение:**
Убедитесь, что вы находитесь в папке CinemaApp при выполнении команд.

### Проблема: Ошибки при сборке

**Решение:**
```powershell
dotnet clean
dotnet restore
dotnet build
```

### Проблема: Библиотеки не загружаются

**Решение:**
1. Убедитесь, что файл `libman.json` существует
2. Выполните `libman restore`
3. Или скачайте библиотеки вручную (см. выше)

## Альтернативный запуск через Visual Studio

1. Откройте файл `CinemaApp.csproj` в Visual Studio 2022
2. Visual Studio автоматически восстановит пакеты
3. Нажмите F5 для запуска

**Требования:**
- Visual Studio 2022 (Community, Professional или Enterprise)
- Рабочая нагрузка "Разработка веб-приложений ASP.NET"

