using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .HasColumnType("UUID")
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(255)
               .HasColumnType("VARCHAR(255)")
               .IsRequired();

        builder.Property(u => u.ContactEmail)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired(false);

        builder.Property(u => u.ContactPhone)
               .HasColumnType("VARCHAR(20)")
               .HasMaxLength(20)
               .IsRequired(false);

        builder.Property(u => u.PasswordHash)
               .HasColumnType("VARCHAR(255)")
               .IsRequired();

        builder.Property(u => u.CreatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasMany(u => u.SentInvitations)
               .WithOne(i => i.Sender)
               .HasForeignKey(i => i.SenderId)
               .IsRequired();

        builder.HasMany(u => u.ReceivedInvitations)
               .WithOne(i => i.Receiver)
               .HasForeignKey(i => i.ReceiverId)
               .IsRequired();

        builder.HasMany(u => u.SentMessages)
               .WithOne(m => m.Sender)
               .HasForeignKey(m => m.SenderId)
               .IsRequired();

        builder.HasMany(u => u.ReceivedMessages)
               .WithOne(m => m.Receiver)
               .HasForeignKey(m => m.ReceiverId)
               .IsRequired();

        builder.HasIndex(u => u.Email)
               .IsUnique();
    }
}