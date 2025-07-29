using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;
using static StudHunter.DB.Postgres.Models.AchievementTemplate;

namespace StudHunter.API.Common;

public static class FillAchievements
{
    public static void SeedAchievements(StudHunterDbContext context)
    {
        if (!context.AchievementTemplates.Any())
        {
            var templates = new List<AchievementTemplate>
            {
                new() { Id = Guid.NewGuid(), OrderNumber = 1, Name = "Первый шаг", Description = "Заполнил профиль", IconUrl = "/icons/student/01.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 2, Name = "В поисках", Description = "Создал резюме", IconUrl = "/icons/student/02.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 3, Name = "Активный кандидат", Description = "Откликнулся на 3 вакансии", IconUrl = "/icons/student/03.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 4, Name = "Популярный профиль", Description = "Получил 3 приглашения от работодателей", IconUrl = "/icons/student/04.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 5, Name = "Начало пути", Description = "Прошёл первую стажировку", IconUrl = "/icons/student/05.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 6, Name = "Стажировочный мастер", Description = "Прошёл 3 стажировки", IconUrl = "/icons/student/06.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 7, Name = "Первая работа", Description = "Получил первое трудоустройство", IconUrl = "/icons/student/07.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 8, Name = "Стабильность", Description = "Работал 6 месяцев на одном месте", IconUrl = "/icons/student/08.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 9, Name = "Признанный талант", Description = "Получил отзыв от работодателя", IconUrl = "/icons/student/09.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 10, Name = "Карьерный рост", Description = "Получил второе трудоустройство", IconUrl = "/icons/student/10.png", Target = AchievementTarget.Student },
                new() { Id = Guid.NewGuid(), OrderNumber = 11, Name = "Добро пожаловать", Description = "Заполнил профиль компании", IconUrl = "/icons/employer/01.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 12, Name = "Первая вакансия", Description = "Разместил первую вакансию", IconUrl = "/icons/employer/02.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 13, Name = "Популярный работодатель", Description = "Получил 5 откликов на вакансии", IconUrl = "/icons/employer/03.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 14, Name = "Активный рекрутер", Description = "Разместил 5 вакансий", IconUrl = "/icons/employer/04.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 15, Name = "Первый наставник", Description = "Предоставил первую стажировку", IconUrl = "/icons/employer/05.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 16, Name = "Опытный наставник", Description = "Предоставил 3 стажировки", IconUrl = "/icons/employer/06.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 17, Name = "Первый найм", Description = "Нанял первого студента", IconUrl = "/icons/employer/07.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 18, Name = "Команда растёт", Description = "Нанял 3 студентов", IconUrl = "/icons/employer/08.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 19, Name = "Доверие студентов", Description = "Получил отзыв от студента", IconUrl = "/icons/employer/09.png", Target = AchievementTarget.Employer },
                new() { Id = Guid.NewGuid(), OrderNumber = 20, Name = "Лидер найма", Description = "Нанял 5 студентов", IconUrl = "/icons/employer/10.png", Target = AchievementTarget.Employer }

            };

            context.AchievementTemplates.AddRange(templates);
            context.SaveChanges();
        }
    }
}
