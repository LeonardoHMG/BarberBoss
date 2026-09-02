using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.DataAccess.Configurations;

public class BillingConfiguration : IEntityTypeConfiguration<Billing>
{
    public void Configure(EntityTypeBuilder<Billing> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.ServiceDate)
            .IsRequired();

        builder.Property(b => b.ClientName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(b => b.ServiceName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(b => b.Amount)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(b => b.PaymentMethod)
           .IsRequired()
           .HasConversion<string>()
           .HasMaxLength(20);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.Notes)
           .HasMaxLength(500);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
