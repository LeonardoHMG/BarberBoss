using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase
{
    public ResponseRegisterBillingJson Execute(RequestBillingJson request)
    {
        //TO DO VALIDATIONS

        return new ResponseRegisterBillingJson();
    }
}
