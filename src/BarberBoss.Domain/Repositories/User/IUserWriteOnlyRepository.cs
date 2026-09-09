namespace BarberBoss.Domain.Repositories.User;
public interface IUserWriteOnlyRepository
{
    Task Add(Entities.User user);
    Task Delete(Guid id);
}
