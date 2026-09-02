using BarberBoss.Domain.Entities;
using BarberBoss.Infrastructure.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess;
public class BarberBossDbContext: DbContext
{
    public BarberBossDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Billing> Billings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new BillingConfiguration());

        base.OnModelCreating(modelBuilder);
    }

}