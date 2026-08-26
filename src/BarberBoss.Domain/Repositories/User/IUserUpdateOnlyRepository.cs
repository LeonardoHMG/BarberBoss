namespace BarberBoss.Domain.Repositories.User;
public interface IUserUpdateOnlyRepository
{
    Task<Entities.User> GetById(Guid Id);
    void Update(Entities.User user);
}