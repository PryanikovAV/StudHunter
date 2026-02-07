using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudHunter.DB.Postgres.Models;

namespace StudHunter.DB.Postgres.Configurations;

public class OrganizationDetailConfiguration : IEntityTypeConfiguration<OrganizationDetail>
{
    public void Configure(EntityTypeBuilder<OrganizationDetail> builder)
    {
        builder.ToTable("OrganizationDetails");

        builder.HasKey(od => od.Id);
        builder.Property(od => od.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(od => od.Inn)
            .HasColumnType("VARCHAR(12)")
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(od => od.Ogrn)
            .HasColumnType("VARCHAR(15)")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(od => od.Kpp)
            .HasColumnType("VARCHAR(9)")
            .HasMaxLength(9)
            .IsRequired(false);

        builder.Property(od => od.LegalAddress)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(od => od.ActualAddress)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.HasOne(od => od.Employer)
            .WithOne(e => e.OrganizationDetails)
            .HasForeignKey<OrganizationDetail>(od => od.EmployerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(od => od.Inn).IsUnique();
    }
}