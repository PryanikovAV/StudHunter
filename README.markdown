# StudHunter API

Проект на стадии разработки.
- :heavy_check_mark: модели БД их конфигурации
- :heavy_check_mark: модели Dto
- :heavy_check_mark: сервисы
- :heavy_check_mark: контроллеры
- :heavy_check_mark: jwt authentication
- :x: CORS
- :x: Пагинация
- :x: фильтры поиска
- :x: XML документация
- :heavy_check_mark: ~~Swagger~~
- :x: ~~Логика Achievements~~

- :x: Внешние сериализаторы
- :x: подготовка к frontend

## Установка и запуск
1. Клонирование:
```bash
git clone https://github.com/PryanikovAV/StudHunter.git
cd StudHunter
```
2. Подготовка миграций
```bash
dotnet ef migrations add InitialCreate --project StudHunter.DB.Postgres --startup-project StudHunter.API
```
2. Запуск инфраструктуры:
```bash
docker-compose up -d --build
```
3. Проверка работоспособности
```bash
docker ps
```
## Дополнительные команды

- Добавить новую миграцию:
```bash
dotnet ef migrations add <NAME> --project StudHunter.DB.Postgres --startup-project StudHunter.API
```
- Удалить последнюю миграцию:
  ```bash
  dotnet ef migrations remove --project ../StudHunter.DB.Postgres
  ```
## База данных
- **Host:** `localhost`
- **Port:** `5433` (внешний порт для Docker)
- **User/Pass:** `postgres` / `postgres`
- **DB:** `studhunter`
### Очистка всего кеша Docker
```bash
docker system prune -a --volumes
```