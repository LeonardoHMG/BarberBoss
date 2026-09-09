using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Users.ChangePassword;
public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage(ResourceErrorMessages.PASSWORD_REQUIRED);
        RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator<RequestChangePasswordJson>());
    }
}
