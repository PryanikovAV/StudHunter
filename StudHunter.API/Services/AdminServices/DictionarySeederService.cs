using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.API.Services.AdminServices;

public class DictionarySeederService(StudHunterDbContext context)
{
    public async Task SeedAllAsync()
    {
        if (!await context.AdditionalSkills.AnyAsync())
        {
            context.AdditionalSkills.AddRange(
                new AdditionalSkill { Id = Guid.NewGuid(), Name = "C# / .NET" },
                new AdditionalSkill { Id = Guid.NewGuid(), Name = "PostgreSQL" },
                new AdditionalSkill { Id = Guid.NewGuid(), Name = "Docker" },
                new AdditionalSkill { Id = Guid.NewGuid(), Name = "React" }
            );
        }

        if (!await context.Faculties.AnyAsync())
        {
            context.Faculties.AddRange(
                new Faculty { Id = Guid.NewGuid(), Name = "Факультет Информационных Технологий" },
                new Faculty { Id = Guid.NewGuid(), Name = "Инженерно-строительный факультет" },
                new Faculty { Id = Guid.NewGuid(), Name = "Гуманитарный факультет" }
            );
        }

        if (!await context.Specialities.AnyAsync())
        {
            context.Specialities.AddRange(
                new Speciality { Id = Guid.NewGuid(), Name = "Программная инженерия", Code = "09.03.04" },
                new Speciality { Id = Guid.NewGuid(), Name = "Информационные системы и технологии", Code = "09.03.02" },
                new Speciality { Id = Guid.NewGuid(), Name = "Кибербезопасность", Code = "10.03.01" }
            );
        }

        if (!await context.Courses.AnyAsync())
        {
            context.Courses.AddRange(
                new Course { Id = Guid.NewGuid(), Name = "Объектно-ориентированное программирование" },
                new Course { Id = Guid.NewGuid(), Name = "Базы данных" },
                new Course { Id = Guid.NewGuid(), Name = "Архитектура ЭВМ" }
            );
        }

        await context.SaveChangesAsync();
    }
}
