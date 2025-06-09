# StudHunter Database

## Требования
- **.NET SDK**: 9.0
- **Docker Desktop**
- **EF Core CLI**:
  ```bash
  dotnet tool install --global dotnet-ef --version 9.0.5
  ```

## Структура проекта
- `StudHunter.DB.Postgres`: Содержит `StudHunterDbContext`, модели (`Models`) и конфигурации (`Configurations`).
- `StudHunter.Migrations`: Содержит миграции EF Core и `StudHunterContextFactory.cs`.

## 1. Запуск базы данных

### 1.1. Запуск Docker-контейнера
1. В корне проекта запустить контейнер:
   ```bash
   docker-compose up -d
   ```
   - Флаг `-d` запускает контейнер в фоновом режиме.
   - Контейнер: `studhunter`, порт: `5433` (маппится на `5432` внутри).
2. Проверка, что контейнер работает:
   ```bash
   docker ps
   ```

### 1.2. Подключение к базе в PGAdmin (опционально)
- Параметры подключения:
  - Host: `localhost`
  - Port: `5433`
  - Database: `studhunter`
  - Username: `postgres`
  - Password: `postgres`
- Проверьте схему `studhunter` после применения миграций.

## 2. Применение миграций

### 2.1. Первоначальная настройка
Если база новая:
1. Создать миграцию:
   ```bash
   dotnet ef migrations add InitialCreate --project StudHunter.Migrations --startup-project StudHunter.Migrations
   ```
2. Применить миграцию:
   ```bash
   dotnet ef database update --project StudHunter.Migrations --startup-project StudHunter.Migrations
   ```

### 2.2. Обновление структуры базы данных
Если изменены модели (например, добавлена сущность):
1. Обновить `StudHunterDbContext.cs`:
   ```csharp
   public DbSet<Project> новая_сущность { get; set; } = null!;
   ```
2. (Опционально) Добавить конфигурацию в `StudHunter.DB.Postgres\Configurations`.
3. Создать миграцию:
   ```bash
   dotnet ef migrations add AddProjectTable --project StudHunter.Migrations --startup-project StudHunter.Migrations
   ```
4. Применить миграцию:
   ```bash
   dotnet ef database update --project StudHunter.Migrations --startup-project StudHunter.Migrations
   ```
## 3. Остановка базы данных и сохранение данных

### 3.1. Остановка контейнера
1. Остановить контейнер:
   ```bash
   docker-compose stop
   ```
2. Проверка:
   ```bash
   docker ps
   ```

3. Сохранение данных:
   - Данные сохраняются в томе `studhunter-data`.
   - При следующем запуске (`docker-compose up -d`) данные доступны.

### 3.2. Полное удаление
Если нужно очистить базу:
```bash
docker-compose down
docker volume rm studhunter-data
```

## 4. Резервное копирование
- Создание бэкапа:
  ```bash
  docker exec -t studhunter pg_dump -U postgres studhunter > studhunter_backup.sql
  ```
- Восстановление:
  ```bash
  docker exec -i studhunter psql -U postgres -d studhunter < studhunter_backup.sql
  ```