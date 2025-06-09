using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;
namespace StudHunter.DB.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(uc => uc.Email)
               .IsUnique();

        builder.Property(uc => uc.Email)
               .HasColumnType("VARCHAR(255)");

        builder.Property(uc => uc.PasswordHash)
               .HasColumnType("VARCHAR(255)");

        builder.Property(uc => uc.CreatedAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(uc => uc.Role)
               .HasColumnType("INTEGER");

        builder.HasMany(u => u.SentInvitations)
               .WithOne(i => i.Sender)
               .HasForeignKey(i => i.SenderId)
               .IsRequired();

        builder.HasMany(u => u.ReceivedInvitations)
               .WithOne(i => i.Receiver)
               .HasForeignKey(i => i.ReceiverId)
               .IsRequired();
    }
}
