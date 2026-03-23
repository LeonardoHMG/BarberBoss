using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Infrastructure.DataAccess;
using BarberBoss.Infrastructure.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure;
public static class DependencyInjectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        AddDbContext(services);
        AddRepositories(services);
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBillingsRepository, BillingsRepository>();
    }

    private static void AddDbContext(IServiceCollection services)
    {
        services.AddDbContext<BarberBossDbContext>();
    }
}
