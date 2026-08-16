using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Infrastructure.DataAccess;
using CommonTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Test.Resources;

namespace WebApi.Test;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public UserCredentials Barber { get; private set; } = default!;
    public UserCredentials Admin { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test")
            .ConfigureServices(services =>
            {
                var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<BarberBossDbContext>(config =>
                {
                    config.UseInMemoryDatabase("InMemoryDbForTesting");
                    config.UseInternalServiceProvider(provider);
                });

                var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BarberBossDbContext>();
                var passwordEncripter = scope.ServiceProvider.GetRequiredService<IPasswordEncripter>();

                StartDatabase(dbContext, passwordEncripter);
            });
    }

    private void StartDatabase(BarberBossDbContext dbContext, IPasswordEncripter passwordEncripter)
    {
        var barber = UserBuilder.Build();
        Barber = new UserCredentials(name: barber.Name, email: barber.Email, password: barber.PasswordHash);
        barber.PasswordHash = passwordEncripter.Encrypt(barber.PasswordHash);
        dbContext.Users.Add(barber);

        var admin = UserBuilder.Build(Roles.ADMIN);
        Admin = new UserCredentials(name: admin.Name, email: admin.Email, password: admin.PasswordHash);
        admin.PasswordHash = passwordEncripter.Encrypt(admin.PasswordHash);
        dbContext.Users.Add(admin);

        dbContext.SaveChanges();
    }
}
