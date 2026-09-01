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

    public BillingsReadOnlyRepositoryBuilder GetById(User user, Billing? billing)
    {
        if (billing is not null)
            _repository.Setup(repo => repo.GetById(user,  billing.Id)).ReturnsAsync(billing);
        return this;
    }

    public BillingsReadOnlyRepositoryBuilder FilterByWeek(List<Billing> billings)
    {
        _repository
            .Setup(repo => repo.FilterByWeek(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(billings);

        return this;
    }

    public BillingsReadOnlyRepositoryBuilder HasAnyBillingForUser(bool result)
    {
        _repository.Setup(repo => repo.HasAnyBillingForUser(It.IsAny<Guid>())).ReturnsAsync(result);
        return this;
    }

    public Mock<IBillingsReadOnlyRepository> MockRepository => _repository;

    public IBillingsReadOnlyRepository Build() => _repository.Object;
}
