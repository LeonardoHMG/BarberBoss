using BarberBoss.Application.UseCases.Users.Update;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Moq;
using Shouldly;

namespace UseCases.Test.Users.Update;
public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user);

        await useCase.Execute(request);

        repositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u => u.Name == request.Name && u.Email == request.Email)),
            Times.Once);
    }

    [Fact]
    public async Task Success_Keeping_Same_Email()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = user.Email;

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user);

        await useCase.Execute(request);

        repositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u => u.Name == request.Name && u.Email == user.Email)),
            Times.Once);
    }

    [Fact]
    public async Task Error_Email_Already_In_Use()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
      
        var (useCase, repositoryMock) = CreateUseCaseWithMock(user, request.Email);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.NAME_EMPTY);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    private (UpdateUserUseCase UseCase, Mock<IUserUpdateOnlyRepository> RepositoryMock) CreateUseCaseWithMock(User user, string? email = null)
    {
        var updateRepositoryBuilder = new UserUpdateOnlyRepositoryBuilder().GetById(user);
        var readRepository = new UserReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        if (string.IsNullOrWhiteSpace(email) == false)
            readRepository.ExistActiveUserWithEmail(email);

        var useCase = new UpdateUserUseCase(
          loggedUser,
          updateRepositoryBuilder.Build(),
          readRepository.Build(),
          unitOfWork);

        return (useCase, updateRepositoryBuilder.MockRepository);
    }
}
