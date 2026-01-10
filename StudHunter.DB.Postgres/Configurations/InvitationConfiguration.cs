using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.ExpiredAt)
               .HasFilter("\"Status\" = 0");  // Index only for active invitations

        builder.HasIndex(i => new { i.SenderId, i.ReceiverId, i.VacancyId, i.Status })
               .HasFilter("\"Status\" = 0")
               .IsUnique()
               .HasDatabaseName("IX_Unique_Active_Invitation");

        builder.HasIndex(i => new { i.SenderId, i.ReceiverId, i.Status })
               .HasFilter("\"Status\" = 0 AND \"VacancyId\" IS NULL")
               .IsUnique()
               .HasDatabaseName("IX_Unique_Active_General_Offer");

        builder.Property(i => i.Message)
               .HasMaxLength(1000);

        builder.Property(i => i.SnapshotReceiverName)
               .HasMaxLength(255);

        builder.Property(i => i.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(i => i.SenderId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(i => i.ReceiverId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(i => i.VacancyId)
               .HasColumnType("UUID");

        builder.Property(i => i.ResumeId)
               .HasColumnType("UUID");

        builder.Property(i => i.Status)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Invitation.InvitationStatus.Sent);

        builder.Property(i => i.Type)
                .HasColumnType("INTEGER")
                .IsRequired();

        builder.Property(i => i.SnapshotVacancyTitle)
               .HasMaxLength(255);

        builder.Property(i => i.SnapshotSenderName)
               .HasMaxLength(255);

        builder.Property(i => i.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.ExpiredAt)
               .HasColumnType("TIMESTAMPTZ");

        builder.HasOne(i => i.Sender)
               .WithMany(u => u.SentInvitations)
               .HasForeignKey(i => i.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Receiver)
               .WithMany(u => u.ReceivedInvitations)
               .HasForeignKey(i => i.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Vacancy)
               .WithMany(v => v.Invitations)
               .HasForeignKey(i => i.VacancyId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Resume)
               .WithMany(r => r.Invitations)
               .HasForeignKey(i => i.ResumeId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.SenderId);
        builder.HasIndex(i => i.ReceiverId);
        builder.HasIndex(i => i.VacancyId);
        builder.HasIndex(i => i.ResumeId);
    }
}
