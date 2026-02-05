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
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(r => r.Title)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(r => r.Description)
               .HasColumnType("TEXT")
               .HasMaxLength(2500);

        builder.Property(r => r.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(r => r.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(r => r.IsDeleted)
               .HasColumnType("BOOLEAN")
               .HasDefaultValue(false);

        builder.Property(u => u.DeletedAt)
               .HasColumnType("TIMESTAMPTZ")
               .IsRequired(false);

        builder.HasOne(r => r.Student)
               .WithOne(s => s.Resume)
               .HasForeignKey<Resume>(r => r.StudentId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

        builder.HasMany(r => r.AdditionalSkills)
               .WithOne(ras => ras.Resume)
               .HasForeignKey(ras => ras.ResumeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Invitations)
               .WithOne(i => i.Resume)
               .HasForeignKey(i => i.ResumeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.HasIndex(r => r.StudentId)
               .IsUnique();
        builder.HasIndex(r => r.CreatedAt);
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
