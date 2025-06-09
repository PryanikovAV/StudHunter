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

        builder.Property(m => m.EmployerId)
               .HasColumnType("UUID");

        builder.Property(m => m.StudentId)
               .HasColumnType("UUID");

        builder.Property(m => m.SenderId)
               .HasColumnType("UUID");

        builder.Property(m => m.Context)
               .HasColumnType("TEXT");

        builder.Property(m => m.SentAt)
               .HasColumnType("TIMESTAMP")
               .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(m => new { m.EmployerId, m.StudentId });
    }
}
