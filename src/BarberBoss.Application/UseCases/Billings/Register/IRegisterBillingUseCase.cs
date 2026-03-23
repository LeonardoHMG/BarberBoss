using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.Register;
public interface IRegisterBillingUseCase
{
    Task<ResponseRegisterBillingJson> Execute(RequestBillingJson request);
}
