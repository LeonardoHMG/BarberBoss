using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.ServiceDate)
             .NotEmpty().WithMessage(ResourceErrorMessages.SERVICE_DATE_REQUIRED)
             .LessThanOrEqualTo(DateTime.Now.AddMinutes(5))
                 .When(billing => billing.ServiceDate != default, ApplyConditionTo.CurrentValidator)
                 .WithMessage(ResourceErrorMessages.DATE_FUTURE)
             .GreaterThan(DateTime.Now.AddYears(-1))
                 .When(billing => billing.ServiceDate != default, ApplyConditionTo.CurrentValidator)
                 .WithMessage(ResourceErrorMessages.DATE_TOO_OLD);

        RuleFor(billing => billing.ServiceDate.Hour)
            .InclusiveBetween(6, 23).WithMessage(ResourceErrorMessages.SERVICE_HOUR_INVALID);

        RuleFor(billing => billing.ClientName)
            .NotEmpty().WithMessage(ResourceErrorMessages.CLIENT_NAME_REQUIRED)
            .Length(2, 120)
                .When(billing => string.IsNullOrWhiteSpace(billing.ClientName) == false, ApplyConditionTo.CurrentValidator)
                .WithMessage(ResourceErrorMessages.CLIENT_NAME_LENGTH);

        RuleFor(billing => billing.ServiceName)
            .NotEmpty().WithMessage(ResourceErrorMessages.SERVICE_NAME_REQUIRED)
            .Length(2, 120)
                .When(billing => string.IsNullOrWhiteSpace(billing.ServiceName) == false, ApplyConditionTo.CurrentValidator)
                .WithMessage(ResourceErrorMessages.SERVICE_NAME_LENGTH);

        RuleFor(billing => billing.Amount)
            .GreaterThanOrEqualTo(0).WithMessage(ResourceErrorMessages.AMOUNT_NEGATIVE);

        RuleFor(billing => billing.Amount)
            .Equal(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_BE_ZERO)
            .When(billing => billing.Status == PaymentStatus.Canceled);

        RuleFor(billing => billing.Amount)
            .GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUNT_REQUIRED)
            .When(billing => billing.Status != PaymentStatus.Canceled);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage(ResourceErrorMessages.PAYMENT_METHOD_INVALID);

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage(ResourceErrorMessages.STATUS_INVALID);

        RuleFor(billing => billing.Notes)
            .MaximumLength(500).WithMessage(ResourceErrorMessages.NOTES_LENGTH);
    }
}