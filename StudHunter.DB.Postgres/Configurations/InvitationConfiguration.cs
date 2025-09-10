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

        builder.Property(i => i.SenderId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(i => i.ReceiverId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(i => i.VacancyId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(i => i.ResumeId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(i => i.Status)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Invitation.InvitationStatus.Sent)
               .IsRequired();

        builder.Property(i => i.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(i => i.UpdatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

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
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Resume)
               .WithMany(r => r.Invitations)
               .HasForeignKey(i => i.ResumeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.SenderId);
        builder.HasIndex(i => i.ReceiverId);
        builder.HasIndex(i => i.VacancyId);
        builder.HasIndex(i => i.ResumeId);
    }
}
