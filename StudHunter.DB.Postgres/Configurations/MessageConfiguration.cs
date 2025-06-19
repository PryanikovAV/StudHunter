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

        builder.Property(m => m.SenderId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(m => m.ReceiverId)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(m => m.Context)
               .HasColumnType("TEXT")
               .HasMaxLength(1000)
               .HasDefaultValue("")
               .IsRequired();

        builder.Property(m => m.SentAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(m => m.Sender)
               .WithMany(u => u.SentMessages)
               .HasForeignKey(m => m.SenderId)
               .IsRequired();

        builder.HasOne(m => m.Receiver)
               .WithMany(u => u.ReceivedMessages)
               .HasForeignKey(m => m.ReceiverId)
               .IsRequired();

        builder.HasIndex(m => m.SenderId);

        builder.HasIndex(m => m.ReceiverId);

        builder.HasIndex(m => m.SentAt);
    }
}
