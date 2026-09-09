using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Users.UpdateById;
public interface IUpdateUserByIdUseCase
{
    Task Execute(Guid id, RequestUpdateUserByAdminJson request);
}
