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
    public UserCredentials OtherBarber { get; private set; } = default!;
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
        Barber = AddUserToDatabase(dbContext, passwordEncripter);
        OtherBarber = AddUserToDatabase(dbContext, passwordEncripter);
        Admin = AddUserToDatabase(dbContext, passwordEncripter, role: Roles.ADMIN);

        dbContext.SaveChanges();
    }

    private UserCredentials AddUserToDatabase(
        BarberBossDbContext dbContext,
        IPasswordEncripter passwordEncripter,
        string role = Roles.BARBER)
    {
        var result = UserBuilder.Build(role);

        var credentials = new UserCredentials(
            name: result.Name,
            email: result.Email,
            password: result.PasswordHash
        );

        var passwordHash = passwordEncripter.Encrypt(result.PasswordHash);

        User userEntity;

        if (role == Roles.ADMIN)
        {
            userEntity = User.RegisterAdmin(result.Name, result.Email, passwordHash);
        }
        else
        {
            userEntity = User.Register(result.Name, result.Email, passwordHash);
        }

        dbContext.Users.Add(userEntity);

        return credentials;
    }
}
