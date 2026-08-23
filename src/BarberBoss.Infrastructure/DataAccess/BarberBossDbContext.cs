using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess;
public class BarberBossDbContext: DbContext
{
    public BarberBossDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Billing> Billings { get; set; }
    public DbSet<User> Users { get; set; }

}