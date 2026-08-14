using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure.Seed;
public static class DatabaseSeeder
{
    public static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var dbContext = serviceProvider.GetRequiredService<BarberBossDbContext>();
        var passwordEncripter = serviceProvider.GetRequiredService<IPasswordEncripter>();

        var hasAnyUser = await dbContext.Users.AnyAsync();

        if (hasAnyUser)
            return;

        var adminPassword = configuration.GetValue<string>("Settings:AdminSeed:Password")
            ?? throw new InvalidOperationException("Senha do admin inicial não configurada.");

        var admin = new User
        {
            Name = "Admin",
            Email = "admin@barberboss.com",
            PasswordHash = passwordEncripter.Encrypt(adminPassword),
            Role = Roles.ADMIN
        };

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}
