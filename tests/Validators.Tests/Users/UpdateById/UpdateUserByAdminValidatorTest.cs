using BarberBoss.Application.UseCases.Users.UpdateById;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;

namespace Validators.Tests.Users.UpdateById;
public class UpdateUserByAdminValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserByAdminValidator();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    public void Error_Name_Empty(string name)
    {
        var validator = new UpdateUserByAdminValidator();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();
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
        var validator = new UpdateUserByAdminValidator();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Email = email;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceErrorMessages.EMAIL_EMPTY);
    }

    [Fact]
    public void Error_Email_Invalid()
    {
        var validator = new UpdateUserByAdminValidator();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Email = "leonardohmg.com";

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceErrorMessages.EMAIL_INVALID);
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData(null)]
    [InlineData("invalid_role")]
    [InlineData("manager")]
    public void Error_Role_Invalid(string role)
    {
        var validator = new UpdateUserByAdminValidator();
        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Role = role;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorMessage.ShouldBe(ResourceErrorMessages.INVALID_ROLE);
    }
}
