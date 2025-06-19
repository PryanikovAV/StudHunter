using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace StudHunter.DB.Postgres.Configurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.HasKey(i  => i.Id);

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
               .HasColumnName("VacancyId")
               .IsRequired(false);

        builder.Property(i => i.ResumeId)
               .HasColumnType("UUID")
               .HasColumnName("ResumeId")
               .IsRequired(false);

        builder.Property(i => i.Type)
               .HasColumnType("INTEGER")
               .IsRequired();

        builder.Property(i => i.Message)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired(false);

        builder.Property(i => i.Status)
               .HasColumnType("INTEGER")
               .HasDefaultValue(Invitation.InvitationStatus.Sent)
               .IsRequired();

        builder.Property(i => i.CreatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(i => i.UpdatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(i => i.Sender)
               .WithMany(u => u.SentInvitations)
               .HasForeignKey(i => i.SenderId)
               .IsRequired();

        builder.HasOne(i => i.Receiver)
               .WithMany(u => u.ReceivedInvitations)
               .HasForeignKey(i => i.ReceiverId)
               .IsRequired();

        builder.HasOne(i => i.Vacancy)
               .WithMany(v => v.Invitations)
               .HasForeignKey(i => i.VacancyId)
               .IsRequired(false);

        builder.HasOne(i => i.Resume)
               .WithMany(r => r.Invitations)
               .HasForeignKey(i => i.ResumeId)
               .IsRequired(false);

        builder.HasIndex(i => new
        {
            i.SenderId,
            i.ReceiverId,
            i.VacancyId
        })
            .IsUnique()
            .HasFilter("\"VacancyId\" is not NULL");

        builder.HasIndex(i => new
        {
            i.SenderId,
            i.ReceiverId,
            i.ResumeId
        })
            .IsUnique()
            .HasFilter("\"ResumeId\" IS NOT NULL");
    }
}
