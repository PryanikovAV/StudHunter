using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.RegistrationStage)
               .HasColumnType("INTEGER")
               .HasDefaultValue(User.AccountStatus.Anonymous)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(u => u.Id)
               .HasColumnType("UUID")
               .HasDefaultValueSql("gen_random_uuid()")
               .IsRequired();

        builder.Property(u => u.Email)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasColumnType("VARCHAR(255)")
               .IsRequired();

        builder.Property(u => u.ContactEmail)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired(false);

        builder.Property(u => u.ContactPhone)
               .HasColumnType("VARCHAR(20)")
               .HasMaxLength(20)
               .HasConversion(
                   v => v == null ? null : new string(v.Where(char.IsDigit).ToArray()),
                   v => v
               )
               .IsRequired(false);

        builder.Property(s => s.AvatarUrl)
               .HasColumnType("VARCHAR(255)")
               .HasMaxLength(255)
               .IsRequired(false);

        builder.Property(u => u.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(u => u.IsDeleted)
               .HasColumnType("BOOLEAN")
               .HasDefaultValue(false)
               .IsRequired();

        builder.Property(u => u.DeletedAt)
               .HasColumnType("TIMESTAMPTZ")
               .IsRequired(false);

        ConfigureRelationships(builder);

        builder.HasQueryFilter(u => !u.IsDeleted);
        builder.HasIndex(u => u.Email).IsUnique();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.BlackLists)
               .WithOne(b => b.User)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ChatsAsUser1)
               .WithOne(c => c.User1)
               .HasForeignKey(c => c.User1Id)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ChatsAsUser2)
               .WithOne(c => c.User2)
               .HasForeignKey(c => c.User2Id)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SentInvitations)
               .WithOne(i => i.Sender)
               .HasForeignKey(i => i.SenderId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ReceivedInvitations)
               .WithOne(i => i.Receiver)
               .HasForeignKey(i => i.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Favorites)
               .WithOne(f => f.User)
               .HasForeignKey(f => f.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
