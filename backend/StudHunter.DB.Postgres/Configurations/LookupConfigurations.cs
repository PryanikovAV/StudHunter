using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Name).HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            new City { Id = SeedIds.CityChelyabinsk, Name = "Челябинск" },
            new City { Id = SeedIds.CityEkaterinburg, Name = "Екатеринбург" }
        );
    }
}

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.Property(u => u.Abbreviation).HasColumnType("VARCHAR(50)").HasMaxLength(50);
        builder.HasIndex(u => u.Name).IsUnique();

        builder.HasData(
            new { Id = SeedIds.UniSUSU, Name = "Южно-Уральский государственный университет", Abbreviation = "ЮУрГУ" },
            new { Id = SeedIds.UniCSU, Name = "Челябинский государственный университет", Abbreviation = "ЧелГУ" },
            new { Id = SeedIds.UniCHGIK, Name = "Челябинский государственный институт культуры", Abbreviation = "ЧГиК" }
        );
    }
}

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(f => f.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.Property(f => f.Description).HasColumnType("TEXT").HasMaxLength(1000);
        builder.HasIndex(f => f.Name).IsUnique();

        builder.HasData(
            new Faculty { Id = SeedIds.FacVshEKN, Name = "Высшая школа электроники и компьютерных наук", Description = "ВШЭКН ЮУрГУ" },
            new Faculty { Id = SeedIds.FacIETN, Name = "Институт естественных и точных наук", Description = "ИЕТН" },
            new Faculty { Id = SeedIds.FacCulture, Name = "Факультет культурологии", Description = "ЧГиК" }
        );
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(d => d.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.HasIndex(d => d.Name).IsUnique();

        builder.HasData(
            new { Id = SeedIds.DepIVT, Name = "Информационно-измерительная техника" },
            new { Id = SeedIds.DepPMI, Name = "Прикладная математика и информатика" }
        );
    }
}

public class StudyDirectionConfiguration : IEntityTypeConfiguration<StudyDirection>
{
    public void Configure(EntityTypeBuilder<StudyDirection> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.Property(s => s.Code).HasColumnType("VARCHAR(20)").HasMaxLength(20);
        builder.Property(s => s.Description).HasColumnType("TEXT").HasMaxLength(1000);
        builder.HasIndex(s => s.Name).IsUnique();

        builder.HasData(
            new StudyDirection { Id = SeedIds.DirSoftwareEng, Name = "Программная инженерия", Code = "09.03.04" },
            new StudyDirection { Id = SeedIds.DirInfSecurity, Name = "Информационная безопасность", Code = "10.05.03" }
        );
    }
}

public class AdditionalSkillConfiguration : IEntityTypeConfiguration<AdditionalSkill>
{
    public void Configure(EntityTypeBuilder<AdditionalSkill> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.HasIndex(s => s.Name).IsUnique();

        builder.HasData(
            new AdditionalSkill { Id = SeedIds.SkillCSharp, Name = "C#" },
            new AdditionalSkill { Id = SeedIds.SkillSql, Name = "PostgreSQL" },
            new AdditionalSkill { Id = SeedIds.SkillReact, Name = "Vue.js" }
        );
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Name).HasColumnType("VARCHAR(255)").HasMaxLength(255).IsRequired();
        builder.Property(c => c.Description).HasColumnType("TEXT").HasMaxLength(1000);
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            new Course { Id = SeedIds.CourseOop, Name = "Объектно-ориентированное программирование" },
            new Course { Id = SeedIds.CourseDb, Name = "Базы данных" }
        );
    }
}