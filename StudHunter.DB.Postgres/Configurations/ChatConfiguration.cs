using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.User1Id)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(c => c.User2Id)
               .HasColumnType("UUID")
               .IsRequired();

        builder.Property(c => c.CreatedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.Property(c => c.LastMessageAt)
               .HasColumnType("TIMESTAMPTZ")
               .IsRequired(false);

        builder.HasOne(c => c.User1)
               .WithMany(u => u.ChatsAsUser1)
               .HasForeignKey(c => c.User1Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.User2)
               .WithMany(u => u.ChatsAsUser2)
               .HasForeignKey(c => c.User2Id)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.User1Id, c.User2Id })
               .IsUnique();
    }
}
