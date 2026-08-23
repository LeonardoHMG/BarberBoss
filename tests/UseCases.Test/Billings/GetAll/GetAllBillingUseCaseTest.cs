using BarberBoss.Application.UseCases.Billings.GetAll;
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

namespace UseCases.Test.Billings.GetAll;
public class GetAllBillingUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var loggedUser = UserBuilder.Build();
        var billings = BillingBuilder.Collection(loggedUser, count: 3);

        var useCase = CreateUseCase(loggedUser, billings);

        var request = RequestGetBillingsJsonBuilder.Build();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Billings.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
        result.Billings.ShouldAllBe(b => billings.Select(x => x.Id).Contains(b.Id));
    }

    [Fact]
    public async Task Success_Calculates_TotalPages_Correctly()
    {
        var loggedUser = UserBuilder.Build();
        var billings = BillingBuilder.Collection(loggedUser, count: 10);

        var readRepository = new BillingsReadOnlyRepositoryBuilder()
            .GetAll(loggedUser, billings, totalCount: 25)
            .Build();

        var mapper = MapperBuilder.Build();
        var loggedUserService = LoggedUserBuilder.Build(loggedUser);

        var useCase = new GetAllBillingUseCase(readRepository, mapper, loggedUserService);

        var request = RequestGetBillingsJsonBuilder.Build();
        request.PageNumber = 1;
        request.PageSize = 10;

        var result = await useCase.Execute(request);

        result.TotalCount.ShouldBe(25);
        result.TotalPages.ShouldBe(3);
        result.CurrentPage.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Fact]
    public async Task Success_Calls_Repository_With_LoggedUser()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();

        var repositoryMock = new Mock<IBillingsReadOnlyRepository>();
        repositoryMock
            .Setup(repo => repo.GetAll(It.IsAny<User>(), It.IsAny<BillingFilter>()))
            .ReturnsAsync((new List<Billing>(), 0));

        var mapper = MapperBuilder.Build();
        var loggedUserService = LoggedUserBuilder.Build(loggedUser);

        var useCase = new GetAllBillingUseCase(repositoryMock.Object, mapper, loggedUserService);

        await useCase.Execute(request);

        repositoryMock.Verify(
            repo => repo.GetAll(It.Is<User>(u => u.Id == loggedUser.Id), It.IsAny<BillingFilter>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Error_PageNumber_Invalid(int pageNumber)
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.PageNumber = pageNumber;

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.PAGE_NUMBER_INVALID);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Error_PageSize_Invalid(int pageSize)
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.PageSize = pageSize;

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.PAGE_SIZE_INVALID);
    }

    [Fact]
    public async Task Error_StartDate_After_EndDate()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.StartDate = DateOnly.FromDateTime(DateTime.Today);
        request.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5));

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.START_DATE_AFTER_END_DATE);
    }

    [Fact]
    public async Task Error_MinAmount_Greater_Than_MaxAmount()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.MinAmount = 500;
        request.MaxAmount = 100;

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.MIN_AMOUNT_GREATER_THAN_MAX_AMOUNT);
    }

    [Fact]
    public async Task Error_MinAmount_Negative()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.MinAmount = -10;

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.MIN_AMOUNT_NEGATIVE);
    }

    [Fact]
    public async Task Error_MaxAmount_Negative()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.MaxAmount = -10;

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.MAX_AMOUNT_NEGATIVE);
    }

    [Fact]
    public async Task Error_ClientName_Search_Too_Short()
    {
        var loggedUser = UserBuilder.Build();
        var request = RequestGetBillingsJsonBuilder.Build();
        request.ClientName = "Jo";

        var useCase = CreateUseCase(loggedUser, new List<Billing>());

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.CLIENT_NAME_SEARCH_LENGTH);
    }

    private GetAllBillingUseCase CreateUseCase(User user, List<Billing> billings)
    {
        var readRepository = new BillingsReadOnlyRepositoryBuilder()
            .GetAll(user, billings)
            .Build();

        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new GetAllBillingUseCase(readRepository, mapper, loggedUser);
    }
}
