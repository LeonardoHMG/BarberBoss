namespace BarberBoss.Application.UseCases.Users.Delete;
public interface IDeleteUserAccountUseCase
{
    Task Execute(Guid id);
}
