using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingsReadOnlyRepository
{
    Task<(List<Billing> Items, int TotalCount)> GetAll(Entities.User user, BillingFilter filter);
    Task<Billing?> GetById(Entities.User user, Guid id);
    Task<bool> Exists(Guid userId, string clientName, string serviceName, DateTime serviceDate);
    Task<bool> HasAnyBillingForUser(Guid userId);
    Task<List<Billing>> FilterByWeek(DateTime startDate, DateTime endDate);
}
