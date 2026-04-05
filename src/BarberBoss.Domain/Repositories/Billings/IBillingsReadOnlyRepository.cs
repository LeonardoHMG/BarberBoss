using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingsReadOnlyRepository
{
    Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter);
    Task<Billing?> GetById(Guid id);
    Task<bool> Exists(string barberName, string clientName, string serviceName, DateOnly date);
    Task<List<Billing>> FilterByWeek(DateOnly startDate, DateOnly endDate);
}
