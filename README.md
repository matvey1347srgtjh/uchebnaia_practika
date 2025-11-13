# Система управления кинотеатром

Учебный проект на ASP.NET Core для управления кинотеатром с функционалом бронирования билетов.

## Функциональность

### Для пользователей:
- Просмотр списка фильмов в прокате
- Просмотр детальной информации о фильме и доступных сеансах
- Бронирование мест с визуализацией схемы зала
- Временная блокировка мест на 5 минут
- Имитация процесса оплаты
- Генерация электронных билетов
- Личный кабинет с историей покупок
- Смена пароля

### Для администраторов:
- Управление фильмами (CRUD)
- Управление залами (CRUD)
- Управление сеансами (CRUD)

## Технологии

- **Backend**: ASP.NET Core 8.0, C#
- **База данных**: SQLite
- **Аутентификация**: ASP.NET Identity
- **Frontend**: Razor Pages, Bootstrap 5, JavaScript
- **Архитектура**: Трехзвенная архитектура (Client, Business Logic, Data Access)

## Установка и запуск

### Шаг 1: Установка .NET SDK

**Важно:** Если команда `dotnet --version` не работает, сначала установите .NET 8.0 SDK.

**Быстрая проверка:**
```powershell
# Запустите скрипт проверки
.\CHECK_DOTNET.ps1
```

**Установка:**
- Скачайте с официального сайта: https://dotnet.microsoft.com/download/dotnet/8.0
- Или используйте winget: `winget install Microsoft.DotNet.SDK.8`
- Подробная инструкция: см. [INSTALLATION.md](INSTALLATION.md)

### Шаг 2: Клонирование и настройка

1. Клонируйте репозиторий (если необходимо)
2. Установите клиентские библиотеки:
   - Если у вас установлен LibMan CLI:
     ```bash
     cd CinemaApp
     libman restore
     ```
   - Или установите через Visual Studio: ПКМ на проекте -> Управление клиентскими библиотеками -> Восстановить клиентские библиотеки
   - Или скачайте вручную:
     - jQuery 3.7.1: https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js
     - Bootstrap 5.3.0: https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/css/bootstrap.min.css и js
     - jQuery Validation: https://cdnjs.cloudflare.com/ajax/libs/jquery-validation/1.19.5/jquery.validate.min.js
     - jQuery Validation Unobtrusive: https://cdnjs.cloudflare.com/ajax/libs/jquery-validation-unobtrusive/4.0.0/jquery.validate.unobtrusive.min.js
4. Откройте проект в Visual Studio или через командную строку:
   ```bash
   cd CinemaApp
   dotnet restore
   dotnet build
   dotnet ef database update
   dotnet run
   ```
5. Откройте браузер и перейдите по адресу `https://localhost:5001` или `http://localhost:5000`

## Учетные данные администратора

- **Email**: admin@cinema.com
- **Пароль**: admin123

## Структура проекта

```
CinemaApp/
├── Controllers/          # Контроллеры MVC
├── Models/              # Модели данных
├── Views/               # Представления Razor
├── Repositories/        # Репозитории (Data Access Layer)
├── Services/            # Сервисы бизнес-логики
├── Data/                # DbContext и SeedData
├── ViewModels/          # Модели представления
└── wwwroot/             # Статические файлы (CSS, JS)
```

## База данных

База данных SQLite создается автоматически при первом запуске приложения. Файл базы данных: `CinemaApp.db`

При первом запуске автоматически создаются:
- Роль "Admin"
- Пользователь-администратор
- Тестовые данные (фильмы, залы, места)

## Особенности реализации

1. **Временная блокировка мест**: При бронировании места блокируются на 5 минут. Фоновый сервис автоматически очищает истекшие резервации.
2. **Визуализация зала**: Интерактивная схема зала с цветовой индикацией состояния мест (свободно, забронировано, занято).
3. **Генерация билетов**: Уникальный код билета генерируется автоматически при оплате.

## Тестирование

Для запуска unit-тестов:
```bash
dotnet test
```

## Лицензия

Учебный проект.

