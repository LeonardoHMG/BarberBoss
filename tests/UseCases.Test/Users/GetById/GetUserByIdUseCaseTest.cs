using BarberBoss.Application.UseCases.Users.GetById;
using BarberBoss.Domain.Entities;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using Shouldly;

namespace UseCases.Test.Users.GetById;
public class GetUserByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(user.Id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
        result.Name.ShouldBe(user.Name);
        result.Email.ShouldBe(user.Email);
        result.Role.ShouldBe(user.Role);
        result.IsActive.ShouldBe(user.IsActive);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        var useCase = CreateUseCase(user: null);

        var act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.USER_NOT_FOUND);
    }

    private GetUserByIdUseCase CreateUseCase(User? user)
    {
        var repositoryBuilder = new UserReadOnlyRepositoryBuilder().GetById(user);
        var mapper = MapperBuilder.Build();

        return new GetUserByIdUseCase(repositoryBuilder.Build(), mapper);
    }
}
