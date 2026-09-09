using BarberBoss.Application.UseCases.Users.UpdateById;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Moq;
using Shouldly;

namespace UseCases.Test.Users.UpdateById;

public class UpdateUserByIdUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var (useCase, updateRepositoryMock) = CreateUseCaseWithMock(user);

        await useCase.Execute(user.Id, request);

        updateRepositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u =>
                u.Name == request.Name &&
                u.Email == request.Email &&
                u.Role == request.Role &&
                u.IsActive == request.IsActive)),
            Times.Once);
    }

    [Fact]
    public async Task Success_Updates_UpdatedAt()
    {
        var user = UserBuilder.Build();
        var originalUpdatedAt = user.UpdatedAt;
        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var (useCase, updateRepositoryMock) = CreateUseCaseWithMock(user);

        await Task.Delay(10);

        await useCase.Execute(user.Id, request);

        updateRepositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u => u.UpdatedAt > originalUpdatedAt)),
            Times.Once);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var (useCase, updateRepositoryMock) = CreateUseCaseWithMock(userToUpdate: null);

        var act = async () => await useCase.Execute(Guid.NewGuid(), request);

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.USER_NOT_FOUND);

        updateRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var user = UserBuilder.Build();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Name = string.Empty;

        var (useCase, updateRepositoryMock) = CreateUseCaseWithMock(user);

        var act = async () => await useCase.Execute(user.Id, request);

        var exception = await Should.ThrowAsync<ErrorOnValidationException>(act);
        exception.GetErrors().Count.ShouldBe(1);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.NAME_EMPTY);

        updateRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Error_Email_Already_Exists()
    {
        var userToUpdate = UserBuilder.Build();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var (useCase, updateRepositoryMock) = CreateUseCaseWithMock(userToUpdate, email: request.Email);

        var act = async () => await useCase.Execute(userToUpdate.Id, request);

        var exception = await Should.ThrowAsync<ConflictException>(act);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);

        updateRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
    }

    private (UpdateUserByIdUseCase UseCase, Mock<IUserUpdateOnlyRepository> UpdateRepositoryMock) CreateUseCaseWithMock(
        User? userToUpdate,
        string? email = null)
    {
        var updateRepositoryBuilder = new UserUpdateOnlyRepositoryBuilder();

        if (userToUpdate is not null)
            updateRepositoryBuilder.GetById(userToUpdate);

        var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder();

        if (string.IsNullOrWhiteSpace(email) == false)
            readRepositoryBuilder.ExistActiveUserWithEmail(email);

        var unitOfWork = UnitOfWorkBuilder.Build();

        var useCase = new UpdateUserByIdUseCase(
            updateRepositoryBuilder.Build(),
            readRepositoryBuilder.Build(),
            unitOfWork);

        return (useCase, updateRepositoryBuilder.MockRepository);
    }
}