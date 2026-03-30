using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsReadOnlyRepository, IBillingsWriteOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Billing billing)
    {
        await _dbContext.Billings.AddAsync(billing);
    }

    public async Task<bool> Delete(Guid id)
    {
        var result = await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);

        if (result is null)
        {
            return false;
        }

        _dbContext.Billings.Remove(result);

        return true;
    }

    public async Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter)
    {
        var query = _dbContext.Billings.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.BarberName))
            query = query.Where(b => b.BarberName.Contains(filter.BarberName));

        if (!string.IsNullOrWhiteSpace(filter.ServiceName))
            query = query.Where(b => b.ServiceName.ToLower().Contains(filter.ServiceName.ToLower()));

        if (filter.StartDate.HasValue)
            query = query.Where(b => b.Date >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(b => b.Date <= filter.EndDate.Value);

        var totalCount = await query.CountAsync();

        query = filter.OrderBy.ToLower() switch
        {
            "amount" => filter.IsDescending ? query.OrderByDescending(b => b.Amount) : query.OrderBy(b => b.Amount),
            "clientname" => filter.IsDescending ? query.OrderByDescending(b => b.ClientName) : query.OrderBy(b => b.ClientName),
            "servicename" => filter.IsDescending ? query.OrderByDescending(b => b.ServiceName) : query.OrderBy(b => b.ServiceName),
            _ => filter.IsDescending ? query.OrderByDescending(b => b.Date) : query.OrderBy(b => b.Date)
        };

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Billing?> GetById(Guid id)
    {
        return await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);
    }
}
