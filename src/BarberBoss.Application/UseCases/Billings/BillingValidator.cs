using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;
public class BillingValidator : AbstractValidator<RequestBillingJson>
{
    public BillingValidator()
    {
        RuleFor(billing => billing.Date)
            .NotEmpty().WithMessage("A data do faturamento é obrigatória.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("A data não pode ser uma data futura")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))).WithMessage("Não é permitido registros com mais de um ano.");

        RuleFor(billing => billing.BarberName)
            .NotEmpty().WithMessage("O nome do barbeiro é obrigatório.")
            .Length(2, 80).WithMessage("O nome do barbeiro deve ter entre 2 e 80 caracteres.");

        RuleFor(billing => billing.ClientName)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
            .Length(2, 120).WithMessage("O nome do cliente deve ter entre 2 e 120 caracteres.");

        RuleFor(billing => billing.ServiceName)
            .NotEmpty().WithMessage("O nome do serviço é obrigatório.")
            .Length(2, 120).WithMessage("O nome do serviço deve ter entre 2 e 120 caracteres.");

        RuleFor(billing => billing.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("O valor não pode ser negativo.");

        RuleFor(billing => billing.Amount)
            .Equal(0).WithMessage("Para cancelados, o valor deve ser 0.00.")
            .When(billing => billing.Status == PaymentStatus.Canceled);
        
        RuleFor(billing => billing.Amount)
            .GreaterThan(0).WithMessage("O valor é obrigatório.")
            .When(billing => billing.Status != PaymentStatus.Canceled);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage("Escolha um método de pagamento válido (Cartão, Dinheiro, Pix ou Outro).");

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage("O status deve ser Pago ou Cancelado.");

        RuleFor(billing => billing.Notes)
            .MaximumLength(500).WithMessage("Observações não podem exceder 500 caracteres.");
    }
}
