using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;
public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.Date)
            .NotEmpty().WithMessage(ResourceErrorMessages.DATE_REQUIRED)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage(ResourceErrorMessages.DATE_FUTURE)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))).WithMessage(ResourceErrorMessages.DATE_TOO_OLD);

        RuleFor(billing => billing.BarberName)
            .NotEmpty().WithMessage(ResourceErrorMessages.BARBER_NAME_REQUIRED)
            .Length(2, 80).WithMessage(ResourceErrorMessages.BARBER_NAME_LENGTH);

        RuleFor(billing => billing.ClientName)
            .NotEmpty().WithMessage(ResourceErrorMessages.CLIENT_NAME_REQUIRED)
            .Length(2, 120).WithMessage(ResourceErrorMessages.CLIENT_NAME_LENGTH);

        RuleFor(billing => billing.ServiceName)
            .NotEmpty().WithMessage(ResourceErrorMessages.SERVICE_NAME_REQUIRED)
            .Length(2, 120).WithMessage(ResourceErrorMessages.SERVICE_NAME_LENGTH);

        RuleFor(billing => billing.Amount)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceErrorMessages.AMOUNT_NEGATIVE);

        RuleFor(billing => billing.Amount)
            .Equal(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_BE_ZERO)
            .When(billing => billing.Status == Status.Canceled);
        
        RuleFor(billing => billing.Amount)
            .GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUNT_REQUIRED)
            .When(billing => billing.Status != Status.Canceled);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_METHOD_INVALID);

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage(ResourceErrorMessages.STATUS_INVALID);

        RuleFor(billing => billing.Notes)
            .MaximumLength(500).WithMessage(ResourceErrorMessages.NOTES_LENGTH);
    }
}
