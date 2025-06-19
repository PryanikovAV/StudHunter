using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
using static StudHunter.DB.Postgres.Models.AchievementTemplate;
namespace StudHunter.DB.Postgres.Configurations;

public class AchievementTemplateConfiguration : IEntityTypeConfiguration<AchievementTemplate>
{
    public void Configure(EntityTypeBuilder<AchievementTemplate> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
               .HasColumnType("INTEGER")
               .ValueGeneratedOnAdd();

        builder.Property(a => a.Name)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(a => a.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(a => a.Target)
               .HasColumnType("INTEGER")
               .IsRequired();

        var templates = new List<AchievementTemplate>
        {
            new() { Id = 1, Name = "Первая попытка", Description = "Откликнулся на первую вакансию", Target = AchievementTarget.Student },
            new() { Id = 2, Name = "Новый путь", Description = "Начал свою первую стажировку", Target = AchievementTarget.Student },
            new() { Id = 3, Name = "Ступень вверх", Description = "Успешно окончил стажировку", Target = AchievementTarget.Student },
            new() { Id = 4, Name = "Я здесь!", Description = "Полностью заполнил профиль", Target = AchievementTarget.Student },
            new() { Id = 5, Name = "Первый шаг в карьере", Description = "Получил первое трудоустройство", Target = AchievementTarget.Student },
            new() { Id = 6, Name = "Опыт копится III", Description = "Работаю уже 3 месяца", Target = AchievementTarget.Student },
            new() { Id = 7, Name = "Опыт копится VI", Description = "Работаю уже 6 месяцев", Target = AchievementTarget.Student },
            new() { Id = 8, Name = "Опыт копится IX", Description = "Работаю уже 9 месяцев", Target = AchievementTarget.Student },
            new() { Id = 9, Name = "Опыт копится XII", Description = "Работаю уже 12 месяцев", Target = AchievementTarget.Student },
            new() { Id = 10, Name = "Звезда работодателей", Description = "Получил 10 приглашений от работодателей", Target = AchievementTarget.Student },
            new() { Id = 11, Name = "Профи стажировок", Description = "Прошел 3 разных стажировки", Target = AchievementTarget.Student },
            new() { Id = 12, Name = "Рекомендации", Description = "Получил первый отзыв от работодателя", Target = AchievementTarget.Student },

            new() { Id = 13, Name = "Добро пожаловать!", Description = "Заполнил профиль", Target = AchievementTarget.Employer },
            new() { Id = 14, Name = "Работодатель мечты", Description = "Разместил первую вакансию", Target = AchievementTarget.Employer },
            new() { Id = 15, Name = "Популярность растет", Description = "10 студентов откликнулись на вакансии", Target = AchievementTarget.Employer },
            new() { Id = 16, Name = "Первые шаги в наставничестве", Description = "Предоставил стажировку первому студенту", Target = AchievementTarget.Employer },
            new() { Id = 17, Name = "Опытный наставник", Description = "Предоставил стажировки для 10 студентов", Target = AchievementTarget.Employer },
            new() { Id = 18, Name = "Первая работа", Description = "Взял на работу первого студента", Target = AchievementTarget.Employer },
            new() { Id = 19, Name = "Крупная компания", Description = "Нанял 10 студентов", Target = AchievementTarget.Employer },
            new() { Id = 20, Name = "Активный работодатель", Description = "Разместил 10 вакансий", Target = AchievementTarget.Employer },
            new() { Id = 21, Name = "Идеальная репутация", Description = "Получил 10 положительных отзывов от студентов", Target = AchievementTarget.Employer }
        };

        builder.HasData(templates);

        builder.HasIndex(a => a.Name)
               .IsUnique();

        builder.HasIndex(a => a.Target);
    }
}
