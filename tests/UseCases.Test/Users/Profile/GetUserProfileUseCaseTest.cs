using BarberBoss.Application.UseCases.Users.Profile;
using BarberBoss.Domain.Entities;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using Shouldly;

namespace UseCases.Test.Users.Profile;
public class GetUserProfileUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var useCase = CreateUserCase(user);

        var result = await useCase.Execute();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
        result.Name.ShouldBe(user.Name);
        result.Email.ShouldBe(user.Email);
        result.Role.ShouldBe(user.Role);
    }

    private GetUserProfileUseCase CreateUserCase(User user)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new GetUserProfileUseCase(loggedUser, mapper);
    }
}
