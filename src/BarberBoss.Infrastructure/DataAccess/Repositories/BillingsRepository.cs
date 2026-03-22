using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsRepository
{
    public void Add(Billing billing)
    {
        var dbContext = new BarberBossDbContext();

        dbContext.Billings.Add(billing);

        dbContext.SaveChanges();
    }
}
