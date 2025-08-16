# StudHunter API

Проект на стадии разработки.
- :heavy_check_mark: модели БД их конфигурации
- :heavy_check_mark: модели Dto
- :heavy_check_mark: сервисы
- :heavy_check_mark: контроллеры и эндпоинты
- :heavy_check_mark: XML документация: DTO, сервисы, контроллеры
- :heavy_check_mark: Swagger
- :hourglass_flowing_sand: jwt authentication
- :x: CORS
- :x: Логика Achievements
- :x: Пагинация
- :x: Внешние сериализаторы
- :x: подготовка к frontend

## Требования

- .NET 9.0 SDK
- PostgreSQL 15+
- Visual Studio 2022 (или IDE поддержкой .NET CLI)
- Git

## Установка и запуск

1. GitHub:
   ```bash
   git clone https://github.com/PryanikovAV/StudHunter.git
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
       "DefaultConnection": "Host=localhost;Port=5432;Database=studhunter;Username=postgres;Password=postgres"
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
