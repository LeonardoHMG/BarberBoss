using BarberBoss.Application.UseCases.Billings;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;

namespace Validators.Tests.Billings.Register;
public class RegisterBillingValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(6)]  
    [InlineData(14)]
    [InlineData(23)]
    public void Success_Service_Hour_Valid(int hour)
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();

        var date = DateTime.Now.AddDays(-1);
        request.ServiceDate = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Date_Future()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceDate = DateTime.Now.AddDays(1).Date.AddHours(10);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.DATE_FUTURE));
    }

    [Fact]
    public void Error_Date_Too_Old()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceDate = DateTime.Now.AddYears(-3).Date.AddHours(10);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.DATE_TOO_OLD));
    }

    [Theory]
    [InlineData(5)]  
    [InlineData(0)] 
    public void Error_Service_Hour_Invalid(int hour)
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();

        var date = DateTime.Now.AddDays(-1);
        request.ServiceDate = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.SERVICE_HOUR_INVALID));
    }

    [Theory]
    [InlineData("B")]
    [InlineData("Premium Executive Grooming Experience: Full Haircut, Hot Towel Shave, Beard Sculpture, Charcoal Face Mask and Relaxing Scalp Massage")]
    public void Error_Service_Name_Invalid(string serviceName)
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceName = serviceName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.SERVICE_NAME_LENGTH));
        });
    }

    [Theory]
    [InlineData("C")]
    [InlineData("Sir Alistair Maximilian Montgomery-Cunningham of the Royal Highland Regiment and Hereditary Grand Chancellor of the United Kingdom IV")]
    public void Error_Client_Name_Invalid(string clientName)
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ClientName = clientName;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.CLIENT_NAME_LENGTH));
        });
    }

    [Fact]
    public void Error_Amount_Negative()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Amount = -1;
        
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_NEGATIVE));
    }

    [Fact]
    public void Error_Amount_Required_When_Not_Canceled()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Status = PaymentStatus.Paid; 
        request.Amount = 0;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_REQUIRED));
    }

    [Fact]
    public void Error_Amount_Must_Be_Zero_When_Canceled()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Status = PaymentStatus.Canceled;
        request.Amount = 50; 

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_ZERO));
    }

    [Fact]
    public void Error_Payment_Method_Invalid()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.PaymentMethod = (PaymentMethod)7;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.PAYMENT_METHOD_INVALID));
        });
    }

    [Fact]
    public void Error_Status_Invalid()
    {
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Status = (PaymentStatus)99;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceErrorMessages.STATUS_INVALID));
    }

    [Fact]
    public void Error_Notes_Invalid()
    {
        var notes = new string('a', 501);
        var validator = new BillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Notes = notes;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceErrorMessages.NOTES_LENGTH));
        });
    }
}
