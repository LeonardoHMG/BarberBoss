using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingsRepository
{
    Task Add(Billing billing);
    Task<(List<Billing> Items, int TotalCount)> GetAll(BillingFilter filter);
}
