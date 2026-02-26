using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.Message)
               .HasMaxLength(1000);

        builder.HasIndex(i => i.ExpiredAt)
               .HasFilter("\"Status\" = 0");

        builder.Property(i => i.SnapshotVacancyTitle)
               .HasMaxLength(255);

        builder.Property(i => i.SnapshotEmployerName)
               .HasMaxLength(255);

        builder.Property(i => i.SnapshotStudentName)
               .HasMaxLength(255);

        builder.Property(i => i.Status)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Invitation.InvitationStatus.Sent);

        builder.Property(i => i.Type)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.Property(i => i.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.ExpiredAt)
               .HasColumnType("TIMESTAMPTZ");

        builder.HasIndex(i => i.ExpiredAt)
               .HasFilter("\"Status\" = 0");

        builder.HasIndex(i => new { i.StudentId, i.EmployerId, i.VacancyId, i.Type })
               .HasFilter("\"Status\" = 0")
               .IsUnique()
               .HasDatabaseName("IX_Unique_Active_Invitation");

        builder.HasOne(i => i.Student)
               .WithMany(s => s.Invitations)
               .HasForeignKey(i => i.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Employer)
               .WithMany(e => e.Invitations)
               .HasForeignKey(i => i.EmployerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Vacancy)
               .WithMany(v => v.Invitations)
               .HasForeignKey(i => i.VacancyId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Resume)
               .WithMany(r => r.Invitations)
               .HasForeignKey(i => i.ResumeId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
