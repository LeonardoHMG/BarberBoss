using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Login.DoLogin;
public interface IDoLoginUseCase
{
    Task<ResponseLoggedUserJson> Execute(RequestLoginJson request);
}
