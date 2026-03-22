using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase
{
    public ResponseRegisterBillingJson Execute(RequestBillingJson request)
    {
        Validate(request);

        var entity = new Billing
        {
            Id = Guid.NewGuid(),
            Date = request.Date,
            BarberName = request.BarberName,
            ClientName = request.ClientName,
            ServiceName = request.ServiceName,
            Amount = request.Amount,    
            PaymentMethod = (Domain.Enums.PaymentMethod)request.PaymentMethod,
            Status = (Domain.Enums.PaymentStatus)request.Status,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        return new ResponseRegisterBillingJson();
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new BillingValidator();

        var result = validator.Validate(request);   

        if (result.IsValid ==  false)
        {
            var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
