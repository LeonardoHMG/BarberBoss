namespace BarberBoss.Domain.Repositories;
public interface IUnitOfWork
{
    Task Commit();
}
