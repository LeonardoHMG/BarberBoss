using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.DataAccess.Configurations;
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Role)
           .IsRequired()
           .HasMaxLength(20);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
