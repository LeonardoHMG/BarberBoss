using BarberBoss.Application.UseCases.Users.Delete;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories.User;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Repositories;
using Moq;
using Shouldly;

namespace UseCases.Test.Users.Delete;
public class DeleteUserAccountUseCaseTest
{
    [Fact]
    public async Task Success_Hard_Deletes_User_Without_Billings()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);
        var targetUser = UserBuilder.Build();

        var (useCase, writeRepositoryMock, _) = CreateUseCaseWithMock(admin, targetUser, hasBillings: false);

        await useCase.Execute(targetUser.Id);

        writeRepositoryMock.Verify(repo => repo.Delete(targetUser.Id), Times.Once);
    }

    [Fact]
    public async Task Success_Deactivates_User_With_Billings()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);
        var targetUser = UserBuilder.Build();

        var (useCase, writeRepositoryMock, updateRepositoryMock) = CreateUseCaseWithMock(admin, targetUser, hasBillings: true);

        await useCase.Execute(targetUser.Id);

        updateRepositoryMock.Verify(
            repo => repo.Update(It.Is<User>(u => u.Id == targetUser.Id && u.IsActive == false)),
            Times.Once);

        writeRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Error_Cannot_Delete_Own_Account()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);

        var (useCase, writeRepositoryMock, _) = CreateUseCaseWithMock(admin, admin, hasBillings: false);

        var act = async () => await useCase.Execute(admin.Id);

        var exception = await Should.ThrowAsync<ForbiddenException>(act);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.CANNOT_DELETE_OWN_ACCOUNT);

        writeRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        var admin = UserBuilder.Build(Roles.ADMIN);

        var (useCase, writeRepositoryMock, _) = CreateUseCaseWithMock(admin, targetUser: null, hasBillings: false);

        var act = async () => await useCase.Execute(Guid.NewGuid());

        var exception = await Should.ThrowAsync<NotFoundException>(act);
        exception.GetErrors().ShouldContain(ResourceErrorMessages.USER_NOT_FOUND);

        writeRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
    }

    private (DeleteUserAccountUseCase UseCase, Mock<IUserWriteOnlyRepository> WriteRepositoryMock, Mock<IUserUpdateOnlyRepository> UpdateRepositoryMock)
        CreateUseCaseWithMock(User admin, User? targetUser, bool hasBillings)
    {
        var readRepositoryBuilder = new UserReadOnlyRepositoryBuilder().GetById(targetUser);

        var writeRepositoryMock = new Mock<IUserWriteOnlyRepository>();

        var updateRepositoryBuilder = targetUser is not null
            ? new UserUpdateOnlyRepositoryBuilder().GetById(targetUser)
            : new UserUpdateOnlyRepositoryBuilder();

        var billingsRepositoryBuilder = new BillingsReadOnlyRepositoryBuilder().HasAnyBillingForUser(hasBillings);

        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(admin);

        var useCase = new DeleteUserAccountUseCase(
            readRepositoryBuilder.Build(),
            writeRepositoryMock.Object,
            updateRepositoryBuilder.Build(),
            billingsRepositoryBuilder.Build(),
            unitOfWork,
            loggedUser);

        return (useCase, writeRepositoryMock, updateRepositoryBuilder.MockRepository);
    }
}
