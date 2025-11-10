# Установка клиентских библиотек

## Проблема с LibMan

Если команда `libman restore` не работает или выдает ошибки, используйте один из альтернативных способов ниже.

## Способ 1: Ручная установка (рекомендуется, если LibMan не работает)

### Создайте структуру папок:

```powershell
mkdir -p wwwroot/lib/jquery/dist
mkdir -p wwwroot/lib/bootstrap/dist
mkdir -p wwwroot/lib/jquery-validation/dist
mkdir -p wwwroot/lib/jquery-validation-unobtrusive
```

### Скачайте файлы:

#### jQuery 3.7.1
```powershell
# PowerShell
Invoke-WebRequest -Uri "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js" -OutFile "wwwroot/lib/jquery/dist/jquery.min.js"
```

#### Bootstrap 5.3.0
```powershell
# CSS
Invoke-WebRequest -Uri "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/css/bootstrap.min.css" -OutFile "wwwroot/lib/bootstrap/dist/css/bootstrap.min.css"

# JS
Invoke-WebRequest -Uri "https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/js/bootstrap.bundle.min.js" -OutFile "wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js"
```

#### jQuery Validation 1.19.5
```powershell
Invoke-WebRequest -Uri "https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.5/jquery.validate.min.js" -OutFile "wwwroot/lib/jquery-validation/dist/jquery.validate.min.js"
```

#### jQuery Validation Unobtrusive 4.0.0
```powershell
Invoke-WebRequest -Uri "https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js" -OutFile "wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"
```

## Способ 2: Использование CDN напрямую (для разработки)

Если библиотеки не устанавливаются, можно временно использовать CDN в `_Layout.cshtml`:

Замените строки:
```html
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
```

На:
```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.19.5/jquery.validate.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js"></script>
```

## Способ 3: Использование npm (если установлен Node.js)

```powershell
npm init -y
npm install jquery@3.7.1 bootstrap@5.3.0 jquery-validation@1.19.5 jquery-validation-unobtrusive@4.0.0

# Скопируйте файлы
Copy-Item "node_modules/jquery/dist/jquery.min.js" -Destination "wwwroot/lib/jquery/dist/"
Copy-Item "node_modules/bootstrap/dist/css/bootstrap.min.css" -Destination "wwwroot/lib/bootstrap/dist/css/"
Copy-Item "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js" -Destination "wwwroot/lib/bootstrap/dist/js/"
Copy-Item "node_modules/jquery-validation/dist/jquery.validate.min.js" -Destination "wwwroot/lib/jquery-validation/dist/"
Copy-Item "node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js" -Destination "wwwroot/lib/jquery-validation-unobtrusive/"
```

## Проверка установки

После установки проверьте, что файлы существуют:

```powershell
Test-Path "wwwroot/lib/jquery/dist/jquery.min.js"
Test-Path "wwwroot/lib/bootstrap/dist/css/bootstrap.min.css"
Test-Path "wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js"
Test-Path "wwwroot/lib/jquery-validation/dist/jquery.validate.min.js"
Test-Path "wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js"
```

Все команды должны вернуть `True`.

