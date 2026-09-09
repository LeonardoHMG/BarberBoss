using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;
public class UserUpdateOnlyRepositoryBuilder
{
    private readonly Mock<IUserUpdateOnlyRepository> _repository;

    public UserUpdateOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserUpdateOnlyRepository>();
    }

    public UserUpdateOnlyRepositoryBuilder GetById(User user)
    {
        _repository.Setup(repository => repository.GetById(user.Id)).ReturnsAsync(user);

        return this;
    }

    public Mock<IUserUpdateOnlyRepository> MockRepository => _repository;

    public IUserUpdateOnlyRepository Build() => _repository.Object;
}
