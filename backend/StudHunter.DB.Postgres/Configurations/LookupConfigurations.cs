using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

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
    }
}

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Name).HasColumnType("VARCHAR(100)").HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(new City
        {
            Id = Guid.Parse("8f8e833b-8f9b-4b2a-9e1d-3b5a1f2c4d5e"),  // TODO: для тестов
            Name = "Челябинск"
        });
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
    }
}