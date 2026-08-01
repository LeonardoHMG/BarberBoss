using FluentValidation;
using FluentValidation.Validators;

namespace BarberBoss.Application.UseCases.Users;

public class PasswordValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return false;
    }
}
