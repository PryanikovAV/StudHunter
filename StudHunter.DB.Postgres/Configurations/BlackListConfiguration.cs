using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class BlackListConfiguration : IEntityTypeConfiguration<BlackList>
{
    public void Configure(EntityTypeBuilder<BlackList> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.BlockedAt)
               .HasColumnType("TIMESTAMPTZ")
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .IsRequired();

        builder.HasOne(b => b.User)
               .WithMany(u => u.BlackLists)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.BlockedUser)
               .WithMany()
               .HasForeignKey(b => b.BlockedUserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.UserId, b.BlockedUserId })
               .IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
                "CK_BlackList_NotSelfBlock",
                "\"UserId\" <> \"BlockedUserId\""));
    }
}
