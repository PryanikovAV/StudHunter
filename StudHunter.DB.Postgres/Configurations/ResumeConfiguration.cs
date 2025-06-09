using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.StudentId)
               .HasColumnType("UUID");

        builder.Property(r => r.Title)
               .HasColumnType("VARCHAR(255)");

        builder.Property(r => r.Description)
               .HasColumnType("TEXT");

        builder.Property(r => r.CreatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.UpdatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(r => r.Student)
               .WithOne(s => s.Resume)
               .HasForeignKey<Resume>(r => r.StudentId)
               .IsRequired();

        builder.HasMany(r => r.Favorites)
               .WithOne(f  => f.Resume)
               .HasForeignKey(f => f.TargetId)
               .IsRequired(false);  // because TargetId can refer to Resume or Vacancy 

        builder.HasMany(r => r.Invitations)
               .WithOne(i => i.Resume)
               .HasForeignKey(r => r.ResumeId)
               .IsRequired(false);

        builder.HasIndex(r => r.StudentId)
               .IsUnique();
    }
}
