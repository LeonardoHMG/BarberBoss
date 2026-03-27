using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;
public interface IBillingsWriteOnlyRepository
{
    Task Add(Billing billing);
}
