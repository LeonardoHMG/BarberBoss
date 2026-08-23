using BarberBoss.Application.UseCases.Billings.GetById;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using Moq;
using Shouldly;

namespace UseCases.Test.Billings.GetById;

public class GetBillingByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var billing = BillingBuilder.Build(loggedUser);

        var useCase = CreateUseCase(loggedUser, billing);

        var result = await useCase.Execute(billing.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(billing.Id);
        result.ServiceDate.ShouldBe(billing.ServiceDate);
        result.BarberName.ShouldBe(loggedUser.Name);
        result.ClientName.ShouldBe(billing.ClientName);
        result.ServiceName.ShouldBe(billing.ServiceName);
        result.Amount.ShouldBe(billing.Amount);
        result.PaymentMethod.ShouldBe((BarberBoss.Communication.Enums.PaymentMethod)billing.PaymentMethod);
        result.Status.ShouldBe((BarberBoss.Communication.Enums.PaymentStatus)billing.Status);
        result.Notes.ShouldBe(billing.Notes);
        result.CreatedAt.ShouldBe(billing.CreatedAt);
        result.UpdatedAt.ShouldBe(billing.UpdatedAt);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        var loggedUser = UserBuilder.Build();

        var useCase = CreateUseCase(loggedUser);

        var act = async () => await useCase.Execute(id: Guid.NewGuid());

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.BILLING_NOT_FOUND);
    }

    [Fact]
    public async Task Success_Calls_Repository_With_LoggedUser()
    {
        var loggedUser = UserBuilder.Build();
        var billing = BillingBuilder.Build(loggedUser);

        var (useCase, repositoryMock) = CreateUseCaseWithMock(loggedUser, billing);

        await useCase.Execute(billing.Id);

        repositoryMock.Verify(
            repo => repo.GetById(It.Is<User>(u => u.Id == loggedUser.Id), billing.Id),
            Times.Once);
    }

    private (GetBillingByIdUseCase UseCase, Mock<IBillingsReadOnlyRepository> RepositoryMock) CreateUseCaseWithMock(User user, Billing? billing = null)
    {
        var repositoryBuilder = new BillingsReadOnlyRepositoryBuilder().GetById(user, billing);

        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var useCase = new GetBillingByIdUseCase(repositoryBuilder.Build(), mapper, loggedUser);

        return (useCase, repositoryBuilder.MockRepository);
    }

    private GetBillingByIdUseCase CreateUseCase(User user, Billing? billing = null)
    {
        var repository = new BillingsReadOnlyRepositoryBuilder().GetById(user, billing).Build();

        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new GetBillingByIdUseCase(repository, mapper, loggedUser);
    }
}