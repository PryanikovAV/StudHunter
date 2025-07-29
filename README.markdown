# StudHunter API

Проект на стадии разработки. Реализованы:
- модели БД
- конфигурации
- модели Dto
- сервисы
- контроллеры и эндпоинты
- Swagger

Планируется:
- Логика Achievements
- jwt authentication
- CORS
- ...

## Требования

- .NET 9.0 SDK
- PostgreSQL 15+
- Visual Studio 2022 (или IDE поддержкой .NET CLI)
- Git

## Установка и запуск

1. GitHub:
   ```bash
   git clone https://github.com/your-username/StudHunter.git
   ```

2. Запуск из директории проекта:
   ```bash
   cd StudHunter/StudHunter.API
   ```

3. Установите зависимости:
   ```bash
   dotnet restore
   ```

4. Настройте строку подключения к PostgreSQL в `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=studhunter;Username=postgres;Password=your_password"
     }
   }
   ```

5. Создайте базу данных в PostgreSQL:
   ```sql
   CREATE DATABASE studhunter;
   ```

6. Примените миграции для создания схемы базы данных:
   ```bash
   dotnet ef database update --project ../StudHunter.DB.Postgres
   ```

7. Скомпилируйте проект:
   ```bash
   dotnet build
   ```

8. Запустите приложение:
   ```bash
   dotnet run
   ```

9. Откройте Swagger UI в браузере:
   ```
   http://localhost:5000/swagger/index.html
   ```

## Дополнительные команды

- Добавить новую миграцию:
  ```bash
  dotnet ef migrations add <MigrationName> --project ../StudHunter.DB.Postgres
  ```

- Удалить последнюю миграцию:
  ```bash
  dotnet ef migrations remove --project ../StudHunter.DB.Postgres
  ```