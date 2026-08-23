using BarberBoss.Application.UseCases.Billings.Update;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Moq;
using Shouldly;

namespace UseCases.Test.Billings.Update;
public class UpdateBillingUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var billing = BillingBuilder.Build(loggedUser);
        var request = RequestBillingJsonBuilder.Build("hair and eyebrow");

        var (useCase, repositoryMock) = CreateUseCaseWithMock(loggedUser, billing);

        await useCase.Execute(billing.Id, request);

        repositoryMock.Verify(
            repo => repo.Update(It.Is<Billing>(b => b.ServiceName == "hair and eyebrow")),
            Times.Once);
    }

    [Fact]
    public async Task Success_Updates_UpdatedAt()
    {
        var loggedUser = UserBuilder.Build();
        var billing = BillingBuilder.Build(loggedUser);
        var originalUpdatedAt = billing.UpdatedAt;
        var request = RequestBillingJsonBuilder.Build();

        var (useCase, repositoryMock) = CreateUseCaseWithMock(loggedUser, billing);

        await useCase.Execute(billing.Id, request);

        repositoryMock.Verify(
            repo => repo.Update(It.Is<Billing>(b => b.UpdatedAt > originalUpdatedAt)),
            Times.Once);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestBillingJsonBuilder.Build();

        var (useCase, repositoryMock) = CreateUseCaseWithMock(loggedUser, billing: null);

        var act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.BILLING_NOT_FOUND);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<Billing>()), Times.Never);
    }

    [Fact]
    public async Task Error_ServiceName_Empty()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var (useCase, repositoryMock) = CreateUseCaseWithMock(loggedUser, billing: null);

        var act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.SERVICE_NAME_REQUIRED);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<Billing>()), Times.Never);
    }

    private (UpdateBillingUseCase UseCase, Mock<IBillingsUpdateOnlyRepository> RepositoryMock) CreateUseCaseWithMock(User user, Billing? billing)
    {
        var repositoryBuilder = new BillingsUpdateOnlyRepositoryBuilder().GetById(user, billing);
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var useCase = new UpdateBillingUseCase(mapper, unitOfWork, repositoryBuilder.Build(), loggedUser);

        return (useCase, repositoryBuilder.MockRepository);
    }
}
