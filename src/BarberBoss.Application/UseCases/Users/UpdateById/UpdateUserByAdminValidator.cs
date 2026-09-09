using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Enums;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Users.UpdateById;
public class UpdateUserByAdminValidator : AbstractValidator<RequestUpdateUserByAdminJson>
{
    public UpdateUserByAdminValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .WithMessage(ResourceErrorMessages.NAME_EMPTY);

        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage(ResourceErrorMessages.EMAIL_EMPTY)
            .EmailAddress()
            .When(user => string.IsNullOrWhiteSpace(user.Email) == false, ApplyConditionTo.CurrentValidator)
            .WithMessage(ResourceErrorMessages.EMAIL_INVALID);

        RuleFor(user => user.Role)
            .Must(role => role == Roles.ADMIN || role == Roles.BARBER)
            .WithMessage(ResourceErrorMessages.INVALID_ROLE);
    }
}
