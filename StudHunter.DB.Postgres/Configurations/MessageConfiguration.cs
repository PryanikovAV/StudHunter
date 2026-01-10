using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.ChatId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(m => m.SenderId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(m => m.ReceiverId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(m => m.IsRead)
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(m => m.Content)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .IsRequired();

        builder.Property(m => m.InvitationId)
               .HasColumnType("UUID")
               .IsRequired(false);

        builder.Property(m => m.SentAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(m => m.Chat)
               .WithMany(c => c.Messages)
               .HasForeignKey(m => m.ChatId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Sender)
               .WithMany(u => u.SentMessages)
               .HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Invitation)
               .WithMany()
               .HasForeignKey(m => m.InvitationId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.ChatId);
        builder.HasIndex(m => m.SentAt);
        builder.HasIndex(m => new { m.ReceiverId, m.IsRead });
    }
}
