using BarberBoss.Application.UseCases.Billings.Register;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
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

namespace UseCases.Test.Billings.Register;
public class RegisterBillingUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestRegisterBillingJsonBuilder.Build();
        var useCase = CreateUseCase(loggedUser);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.ServiceName.ShouldBe(request.ServiceName);
        result.Amount.ShouldBe(request.Amount);
        result.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task Success_Assigns_LoggedUser_Id()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestRegisterBillingJsonBuilder.Build();

        var (useCase, writeRepositoryMock) = CreateUseCaseWithMock(loggedUser);

        await useCase.Execute(request);

        writeRepositoryMock.Verify(
            repo => repo.Add(It.Is<Billing>(b => b.UserId == loggedUser.Id)),
            Times.Once);
    }

    [Fact]
    public async Task Error_Admin_Cannot_Register()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);
        var request = RequestRegisterBillingJsonBuilder.Build();
        var useCase = CreateUseCase(admin);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ForbiddenException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.ADMIN_CANNOT_REGISTER_BILLING);
    }

    [Fact]
    public async Task Error_Billing_Already_Exists()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestRegisterBillingJsonBuilder.Build();
        var useCase = CreateUseCase(loggedUser, exists: true);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ConflictException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.BILLING_ALREADY_EXISTS);
    }

    [Fact]
    public async Task Error_ServiceName_Empty()
    {
        var loggedUser = UserBuilder.Build();

        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var useCase = CreateUseCase(loggedUser);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.SERVICE_NAME_REQUIRED);
    }

    [Fact]
    public async Task Error_Admin_Cannot_Register_Even_With_Invalid_Data()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceName = string.Empty; 

        var useCase = CreateUseCase(admin);

        var act = async () => await useCase.Execute(request);

        await Should.ThrowAsync<ForbiddenException>(act); 
    }

    private RegisterBillingUseCase CreateUseCase(User user, bool exists = false)
    {
        var writeRepository = new BillingsWriteOnlyRepositoryBuilder().Build();
        var readRepository = new BillingsReadOnlyRepositoryBuilder().Exists(exists).Build();
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new RegisterBillingUseCase(writeRepository, readRepository, unitOfWork, mapper, loggedUser);
    }

    private (RegisterBillingUseCase UseCase, Mock<IBillingsWriteOnlyRepository> WriteRepositoryMock) CreateUseCaseWithMock(User user, bool exists = false)
    {
        var writeRepository = new BillingsWriteOnlyRepositoryBuilder();
        var readRepository = new BillingsReadOnlyRepositoryBuilder().Exists(exists).Build();
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var useCase = new RegisterBillingUseCase(
            writeRepository.Build(),
            readRepository,
            unitOfWork,
            mapper,
            loggedUser);

        return (useCase, writeRepository.MockRepository);
    }
}
