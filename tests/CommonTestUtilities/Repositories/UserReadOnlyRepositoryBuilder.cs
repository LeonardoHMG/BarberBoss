using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories.User;
using Moq;

namespace CommonTestUtilities.Repositories;
public class UserReadOnlyRepositoryBuilder
{
    private readonly Mock<IUserReadOnlyRepository> _repository;

    public UserReadOnlyRepositoryBuilder()
    {
        _repository = new Mock<IUserReadOnlyRepository>();
    }

    public UserReadOnlyRepositoryBuilder ExistActiveUserWithEmail(string email)
    {
        _repository
            .Setup(repository => repository.ExistActiveUserWithEmail(email))
            .ReturnsAsync(true);

        return this;
    }

    public UserReadOnlyRepositoryBuilder GetUserByEmail(User user)
    {
        _repository.Setup(userRepository => userRepository.GetUserByEmail(user.Email)).ReturnsAsync(user);

        return this;
    }

    public UserReadOnlyRepositoryBuilder GetById(User? user)
    {
        if (user is not null)
            _repository.Setup(repo => repo.GetById(user.Id)).ReturnsAsync(user);

        return this;
    }

    public IUserReadOnlyRepository Build() => _repository.Object;
}
