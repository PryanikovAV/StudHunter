### Проект на стадии разработки.
- :heavy_check_mark: модели БД их конфигурации
- :heavy_check_mark: модели Dto
- :heavy_check_mark: сервисы
- :heavy_check_mark: контроллеры и эндпоинты
- :x: XML документация: DTO, сервисы, контроллеры
- :x: OpenApi
- :hourglass_flowing_sand: фильтры поиска
- :heavy_check_mark: jwt authentication
- :heavy_check_mark: CORS
- :heavy_check_mark: пагинация
- :hourglass_flowing_sand: Frontend: Vue
# StudHunter 

**StudHunter** - это специализированная веб-платформа, предназначенная для интеграции студентов в профессиональную среду региональных предприятий. В отличие от массовых рекрутинговых сервисов, система интегрирована в академическую среду вуза и фокусируется на подборе стажировок на основе учебного плана.
## Ключевые особенности
- **Академический профиль:** автоматический учет факультета, курса и пройденных дисциплин при формировании резюме.
- **Многоэтапная активация:** система динамического изменения прав доступа пользователя в зависимости от полноты заполнения профиля.
- **Smart Matching:** фильтрация вакансий и соискателей по конкретным учебным курсам и hard skills.
- **Real-time Communication:** система мгновенных сообщений для прямого взаимодействия студентов и работодателей.
- **Soft Delete & Data Integrity:** логика деактивации аккаунтов с сохранением исторической аналитики для университета.
## Технологический стек
### Backend
- **Framework:** .NET 10 (C#)
- **Database:** PostgreSQL + Entity Framework Core
- **Real-time:** SignalR
- **Security:** JWT Authentication, Role-Based Access Control
### Frontend
- **Framework:** Vue.js 3 (Composition API)
- **State Management:** Pinia    
## Архитектурные решения
- **Table-per-Concrete-Type (TPC):**  стратегия наследования БД для эффективного разделения сущностей пользователей (Студент, Работодатель, Админ).
- **Snapshot Pattern:** сохранение ключевых данных в приглашениях (названия вакансий, имена) для обеспечения исторической достоверности при изменении исходных записей.
- **Service Layer:** вынос сложной бизнес-логики (пересчет статусов, каскадное удаление) в изолированные сервисы.
## Инсталляция и запуск

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