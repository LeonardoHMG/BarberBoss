using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Billing billing)
    {
       _dbContext.Billings.Add(billing);

        _dbContext.SaveChanges();
    }
}
