using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using Moq;

namespace CommonTestUtilities.Repositories;
public class BillingsReadOnlyRepositoryBuilder
{
    private readonly Mock<IBillingsReadOnlyRepository> _repository;

	public BillingsReadOnlyRepositoryBuilder()
	{
		_repository = new Mock<IBillingsReadOnlyRepository>();
	}

    public BillingsReadOnlyRepositoryBuilder Exists(bool result)
    {
        _repository
            .Setup(repo => repo.Exists(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(result);

        return this;
    }

    public BillingsReadOnlyRepositoryBuilder GetAll(User user, List<Billing> billings, int? totalCount = null)
    {
        _repository
            .Setup(repo => repo.GetAll(user, It.IsAny<BillingFilter>()))
            .ReturnsAsync((billings, totalCount ?? billings.Count));

        return this;
    }

    public IBillingsReadOnlyRepository Build() => _repository.Object;
}
