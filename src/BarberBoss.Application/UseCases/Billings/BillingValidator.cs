using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;
public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.Date)
            .NotEmpty().WithMessage("Billing date is required.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Billing date cannot be in the future.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))).WithMessage("Billing cannot be recorded more than one year in the past.");

        RuleFor(billing => billing.BarberName)
            .NotEmpty().WithMessage("Barber's name is required.")
            .Length(2, 80).WithMessage("Barber's name must be between 2 and 80 characters.");

        RuleFor(billing => billing.ClientName)
            .NotEmpty().WithMessage("Client's name is required.")
            .Length(2, 120).WithMessage("Client's name must be between 2 and 120 characters.");

        RuleFor(billing => billing.ServiceName)
            .NotEmpty().WithMessage("Service name is required.")
            .Length(2, 120).WithMessage("Service name must be between 2 and 120 characters.");

        RuleFor(billing => billing.Amount)
            .NotNull().WithMessage("Amount is required.")
            .GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");

        RuleFor(billing => billing.Amount)
            .Equal(0).WithMessage("For 'Canceled' status, the amount must be 0.00.")
            .When(billing => billing.Status == Status.Canceled);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage("Please select a valid payment method (Card, Cash, Pix, or Other).");

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage("Status must be either 'Paid' or 'Canceled'.");

        RuleFor(billing => billing.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}
