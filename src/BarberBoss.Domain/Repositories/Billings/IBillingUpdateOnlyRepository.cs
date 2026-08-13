using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingUpdateOnlyRepository
{
    Task<Billing?> GetById(Entities.User user, Guid id);
    void Update(Billing billing);
}
