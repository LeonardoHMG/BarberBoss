using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Domain.Security.Cryptography;
using BarberBoss.Domain.Security.Tokens;
using BarberBoss.Domain.Services.LoggedUser;
using BarberBoss.Infrastructure.DataAccess;
using BarberBoss.Infrastructure.DataAccess.Repositories;
using BarberBoss.Infrastructure.Extensions;
using BarberBoss.Infrastructure.Security.Tokens;
using BarberBoss.Infrastructure.Services.LoggedUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure;
public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPasswordEncripter, Security.Cryptography.BCrypt>();
        services.AddScoped<ILoggedUser, LoggedUser>();

        AddToken(services, configuration);
        AddRepositories(services);

        if(configuration.IsTestEnvironment() == false)
        {
            AddDbContext(services, configuration);
        }
    }

    private static void AddToken(IServiceCollection services, IConfiguration configuration)
    {
        var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpiresMinutes");

        var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

        services.AddScoped<IAccessTokenGenerator>(config => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBillingsReadOnlyRepository, BillingsRepository>();
        services.AddScoped<IBillingsWriteOnlyRepository, BillingsRepository>();
        services.AddScoped<IBillingsUpdateOnlyRepository, BillingsRepository>();
        services.AddScoped<IUserReadOnlyRepository, UserRepository>();
        services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Connection");

        var serverVersion = ServerVersion.AutoDetect(connectionString);

        services.AddDbContext<BarberBossDbContext>(config => config.UseMySql(connectionString, serverVersion));
    }
}
