using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Users.Update;
public interface IUpdateUserUseCase
{
    Task Execute(RequestUpdateUserJson request);
}
