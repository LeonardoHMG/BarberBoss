using BarberBoss.Application.UseCases.Billings.Delete;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using Moq;
using Shouldly;

namespace UseCases.Test.Billings.Delete;
public class DeleteBillingUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var billing = BillingBuilder.Build(loggedUser);

        var (useCase, writeRepositoryMock) = CreateUseCaseWithMock(loggedUser, billing);

        await useCase.Execute(billing.Id);

        writeRepositoryMock.Verify(repo => repo.Delete(billing.Id), Times.Once);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var loggedUser = UserBuilder.Build();

        var (useCase, writeRepositoryMock) = CreateUseCaseWithMock(loggedUser, billing: null);

        var act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.BILLING_NOT_FOUND);

        writeRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
    }

    private (DeleteBillingUseCase UseCase, Mock<IBillingsWriteOnlyRepository> repository) CreateUseCaseWithMock(User user, Billing? billing)
    {
        var writeRepository = new Mock<IBillingsWriteOnlyRepository>();
        var readRepository = new BillingsReadOnlyRepositoryBuilder().GetById(user, billing).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var useCase = new DeleteBillingUseCase(writeRepository.Object, unitOfWork, loggedUser, readRepository);

        return (useCase, writeRepository);
    }
}
