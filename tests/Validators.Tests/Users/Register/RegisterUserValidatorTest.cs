using BarberBoss.Application.UseCases.Users.Register;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;

namespace Validators.Tests.Users.Register;
public class RegisterUserValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = name;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceErrorMessages.NAME_EMPTY);
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    public void Error_Email_Empty(string email)
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Email = email;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceErrorMessages.EMAIL_EMPTY);
    }
}
