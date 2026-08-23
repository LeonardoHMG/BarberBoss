using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;
public class BillingsUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsUpdateOnlyRepository> _repository;

    public BillingsUpdateOnlyRepositoryBuilder()
    {
        _repository = new Mock<IBillingsUpdateOnlyRepository>();
    }

    public BillingsUpdateOnlyRepositoryBuilder GetById(User user, Billing? billing)
    {
        if (billing is not null)
            _repository.Setup(repository => repository.GetById(user, billing.Id)).ReturnsAsync(billing);

        return this;
    }

    public Mock<IBillingsUpdateOnlyRepository> MockRepository => _repository;

    public IBillingsUpdateOnlyRepository Build() => _repository.Object;
}
