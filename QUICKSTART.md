# Быстрый старт

## Предварительные требования

- .NET 8.0 SDK ([Скачать здесь](https://dotnet.microsoft.com/download/dotnet/8.0))
- Visual Studio 2022 или VS Code (опционально)

**Важно:** Если команда `dotnet --version` не работает, сначала установите .NET SDK. 
См. подробную инструкцию в файле [INSTALLATION.md](INSTALLATION.md)

## Установка

1. **Восстановите NuGet пакеты:**
   ```bash
   cd CinemaApp
   dotnet restore
   ```

2. **Установите клиентские библиотеки (выберите один из способов):**

   **Вариант 1: Автоматический скрипт (рекомендуется, если LibMan не работает)**
   ```powershell
   cd CinemaApp
   .\install-libraries.ps1
   ```

   **Вариант 2: Через LibMan**
   ```bash
   # Установите LibMan CLI, если еще не установлен
   dotnet tool install -g Microsoft.Web.LibraryManager.Cli
   
   # Восстановите библиотеки
   libman restore
   ```
   
   **Если LibMan выдает ошибки**, используйте Вариант 1 или см. [INSTALL_LIBRARIES.md](INSTALL_LIBRARIES.md)

   **Вариант 3: Через Visual Studio**
   - Правый клик на проекте → "Управление клиентскими библиотеками"
   - Нажмите "Восстановить клиентские библиотеки"

   **Вариант 4: Вручную**
   Создайте следующие директории и скачайте файлы:
   ```
   wwwroot/lib/jquery/dist/jquery.min.js
   wwwroot/lib/bootstrap/dist/css/bootstrap.min.css
   wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js
   wwwroot/lib/jquery-validation/dist/jquery.validate.min.js
   wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js
   ```

3. **Запустите приложение:**
   ```bash
   dotnet run
   ```

4. **Откройте браузер:**
   - HTTPS: https://localhost:5001
   - HTTP: http://localhost:5000

## Первый вход

**Администратор:**
- Email: `admin@cinema.com`
- Пароль: `admin123`

**Обычный пользователь:**
- Зарегистрируйтесь через форму регистрации

## Основные функции

1. **Просмотр фильмов** - Главная страница
2. **Бронирование билетов** - Выберите фильм → Сеанс → Места → Оплата
3. **Администрирование** - Войдите как администратор для управления контентом

## База данных

База данных SQLite создается автоматически при первом запуске.
Файл: `CinemaApp.db`

## Тестирование

```bash
cd CinemaApp.Tests
dotnet test
```

