using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase
{
    public ResponseRegisterBillingJson Execute(RequestBillingJson request)
    {
        Validate(request);

        return new ResponseRegisterBillingJson();
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new BillingValidator();

        var result = validator.Validate(request);   

        if (result.IsValid ==  false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ArgumentException();
        }
    }
}
