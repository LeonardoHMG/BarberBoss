using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;
public class BillingsWriteOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsWriteOnlyRepository> _repository;

    public BillingsWriteOnlyRepositoryBuilder()
    {
        _repository = new Mock<IBillingsWriteOnlyRepository>();
    }

    public Mock<IBillingsWriteOnlyRepository> MockRepository => _repository;

    public IBillingsWriteOnlyRepository Build() => _repository.Object;
}
