using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Users.ChangePassword;
public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson request);
}
