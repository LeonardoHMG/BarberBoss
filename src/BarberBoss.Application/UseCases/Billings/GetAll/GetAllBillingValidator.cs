using BarberBoss.Communication.Requests;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings.GetAll;
public class GetAllBillingValidator : AbstractValidator<RequestGetBillingsJson>
{
    public GetAllBillingValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("O número da página deve ser maior ou igual a 1.");

        RuleFor(r => r.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100.");

        When(r => r.StartDate.HasValue && r.EndDate.HasValue, () =>
        {
            RuleFor(r => r.StartDate)
                .LessThanOrEqualTo(r => r.EndDate)
                .WithMessage("A data de início não pode ser posterior à data de término.");
        });
    }
}
