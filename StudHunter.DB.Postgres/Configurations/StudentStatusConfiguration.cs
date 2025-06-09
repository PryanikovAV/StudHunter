using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class StudentStatusConfiguration : IEntityTypeConfiguration<StudentStatus>
{
    public void Configure(EntityTypeBuilder<StudentStatus> builder)
    {
        builder.HasKey(ss => ss.Id);

        builder.Property(ss => ss.Id)
               .HasColumnType("INTEGER");

        builder.Property(ss => ss.Name)
               .HasColumnType("VARCHAR(50)");

        builder.HasData(
            new StudentStatus { Id = 1, Name = "Учусь" },
            new StudentStatus { Id = 2, Name = "В поисках стажировки" },
            new StudentStatus { Id = 3, Name = "В поисках работы" },
            new StudentStatus { Id = 4, Name = "Стажируюсь" },
            new StudentStatus { Id = 5, Name = "Работаю" }
            );
    }
}
