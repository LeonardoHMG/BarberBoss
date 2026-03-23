using BarberBoss.Domain.Repositories;

namespace BarberBoss.Infrastructure.DataAccess;

internal class UnitOfWork : IUnitOfWork
{
    private readonly BarberBossDbContext _dbContext;
    public UnitOfWork(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Commit() => _dbContext.SaveChanges();
}