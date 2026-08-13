using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingsReadOnlyRepository
{
    Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter);
    Task<Billing?> GetById(Guid id);
    Task<bool> Exists(Guid userId, string clientName, string serviceName, DateTime serviceDate);
    Task<List<Billing>> FilterByWeek(DateTime startDate, DateTime endDate);
}
