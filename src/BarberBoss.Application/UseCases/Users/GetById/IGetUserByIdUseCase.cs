using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Users.GetById;
public interface IGetUserByIdUseCase
{
    Task<ResponseUserJson> Execute(Guid id);
}
