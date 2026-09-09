using BarberBoss.Application.UseCases.Users.ChangePassword;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Cryptography;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Moq;
using Shouldly;

namespace UseCases.Test.Users.ChangePassword;
public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user, request.Password);

        await useCase.Execute(request);

        repositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u => u.Id == user.Id)),
            Times.Once);
    }

    [Fact]
    public async Task Error_Current_Password_Invalid()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Error_New_Password_Invalid()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = "weak";

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user, request.Password);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.INVALID_PASSWORD);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Error_Password_Empty()
    {
        var user = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = string.Empty;

        var (useCase, repositoryMock) = CreateUseCaseWithMock(user);

        var act = async () => await useCase.Execute(request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.PASSWORD_REQUIRED);

        repositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    private (ChangePasswordUseCase UseCase, Mock<IUserUpdateOnlyRepository> RepositoryMock) CreateUseCaseWithMock(User user, string? password = null)
    {
        var updateRepositoryBuilder = new UserUpdateOnlyRepositoryBuilder().GetById(user);
        var passwordEncripter = new PasswordEncrypterBuilder().Verify(password).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var useCase = new ChangePasswordUseCase(
            loggedUser,
            passwordEncripter,
            updateRepositoryBuilder.Build(),
            unitOfWork);

        return (useCase, updateRepositoryBuilder.MockRepository);
    }
}
