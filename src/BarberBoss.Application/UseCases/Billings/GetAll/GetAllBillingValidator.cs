using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings.GetAll;
public class GetAllBillingValidator : AbstractValidator<RequestGetBillingsJson>
{
    public GetAllBillingValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(ResourceErrorMessages.PAGE_NUMBER_INVALID);

        RuleFor(r => r.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(ResourceErrorMessages.PAGE_SIZE_INVALID);

        When(r => r.StartDate.HasValue && r.EndDate.HasValue, () =>
        {
            RuleFor(r => r.StartDate)
                .LessThanOrEqualTo(r => r.EndDate)
                .WithMessage(ResourceErrorMessages.START_DATE_AFTER_END_DATE);
        });

        When(r => r.MinAmount.HasValue && r.MaxAmount.HasValue, () =>
        {
            RuleFor(r => r.MinAmount)
                .LessThanOrEqualTo(r => r.MaxAmount)
                .WithMessage(ResourceErrorMessages.MIN_AMOUNT_GREATER_THAN_MAX_AMOUNT);
        });

        RuleFor(r => r.MinAmount)
            .GreaterThanOrEqualTo(0)
            .When(r => r.MinAmount.HasValue)
            .WithMessage(ResourceErrorMessages.MIN_AMOUNT_NEGATIVE);

        RuleFor(r => r.MaxAmount)
            .GreaterThanOrEqualTo(0)
            .When(r => r.MaxAmount.HasValue)
            .WithMessage(ResourceErrorMessages.MAX_AMOUNT_NEGATIVE);

        RuleFor(r => r.ClientName)
            .MinimumLength(3)
            .When(r => !string.IsNullOrWhiteSpace(r.ClientName))
            .WithMessage(ResourceErrorMessages.CLIENT_NAME_SEARCH_LENGTH);
    }
}
