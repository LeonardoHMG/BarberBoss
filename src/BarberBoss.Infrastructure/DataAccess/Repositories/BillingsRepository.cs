using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsReadOnlyRepository, IBillingsWriteOnlyRepository, IBillingUpdateOnlyRepository
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
            query = query.Where(b => b.BarberName.Contains(filter.BarberName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.ServiceName))
            query = query.Where(b => b.ServiceName.ToLower().Contains(filter.ServiceName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.ClientName))
            query = query.Where(b => b.ClientName.Contains(filter.ClientName));

        if (filter.MinAmount.HasValue)
            query = query.Where(b => b.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(b => b.Amount <= filter.MaxAmount.Value);

        if (filter.StartDate.HasValue)
        {
            var start = filter.StartDate.Value.ToDateTime(new TimeOnly(7, 0, 0));
            query = query.Where(b => b.ServiceDate >= start);
        }

        if (filter.EndDate.HasValue)
        {
            var end = filter.EndDate.Value.ToDateTime(new TimeOnly(22, 0, 0));
            query = query.Where(b => b.ServiceDate <= end);
        }

        if (filter.Status.HasValue)
            query = query.Where(b => b.Status == filter.Status.Value);

        if (filter.PaymentMethod.HasValue)
            query = query.Where(b => b.PaymentMethod == filter.PaymentMethod.Value);

        var totalCount = await query.CountAsync();

        query = filter.OrderBy.ToLower() switch
        {
            "amount" => filter.IsDescending ? query.OrderByDescending(b => b.Amount) : query.OrderBy(b => b.Amount),
            "clientname" => filter.IsDescending ? query.OrderByDescending(b => b.ClientName) : query.OrderBy(b => b.ClientName),
            "servicename" => filter.IsDescending ? query.OrderByDescending(b => b.ServiceName) : query.OrderBy(b => b.ServiceName),
            _ => filter.IsDescending ? query.OrderByDescending(b => b.ServiceDate) : query.OrderBy(b => b.ServiceDate)
        };

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    async Task<Billing?> IBillingsReadOnlyRepository.GetById(Guid id)
    {
        return await _dbContext.Billings.AsNoTracking().FirstOrDefaultAsync(billing => billing.Id == id);
    }

    async Task<Billing?> IBillingUpdateOnlyRepository.GetById(Guid id)
    {
        return await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);
    }

    public void Update(Billing billing)
    {
        _dbContext.Billings.Update(billing);
    }

    public async Task<bool> Exists(string barberName, string clientName, string serviceName, DateTime serviceDate)
    {
        var dateOnly = serviceDate.Date;

        return await _dbContext.Billings.AnyAsync(b =>
            b.BarberName.ToLower() == barberName.ToLower() &&
            b.ClientName.ToLower() == clientName.ToLower() &&
            b.ServiceName.ToLower() == serviceName.ToLower() &&
            b.ServiceDate.Date == dateOnly);
    }

    public async Task<List<Billing>> FilterByWeek(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.Billings
            .AsNoTracking()
            .Where(b => b.ServiceDate >= startDate && b.ServiceDate <= endDate && b.Status == PaymentStatus.Paid)
            .OrderBy(b => b.ServiceDate)
            .ToListAsync();
    }
}
