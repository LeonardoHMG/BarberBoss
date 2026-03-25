using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.GetAll;
public interface IGetAllBillingUseCase
{
    Task<ResponseBillingsJson> Execute();
}
