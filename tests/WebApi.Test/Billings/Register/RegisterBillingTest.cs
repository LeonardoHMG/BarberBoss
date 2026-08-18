using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Billings.Register;

public class RegisterBillingTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Billings";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    private static readonly DateTime ValidDate = DateTime.Now.AddDays(-1).Date.AddHours(10);

    public RegisterBillingTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Barber_Can_Register()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestRegisterBillingJsonBuilder.Build("beard");

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("serviceName").GetString().ShouldBe(request.ServiceName);
        response.RootElement.GetProperty("amount").GetDecimal().ShouldBe(request.Amount);
    }

    [Fact]
    public async Task Error_Admin_Cannot_Register()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestRegisterBillingJsonBuilder.Build("hair");

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.ADMIN_CANNOT_REGISTER_BILLING);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestRegisterBillingJsonBuilder.Build("hair and beard");

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }

    [Fact]
    public async Task Error_ServiceName_Empty()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceName = string.Empty;

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.SERVICE_NAME_REQUIRED);
    }
    [Fact]
    public async Task Error_Billing_Already_Exists()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestRegisterBillingJsonBuilder.Build("hair and eyebrow");

        var firstResult = await _httpClient.PostAsJsonAsync(METHOD, request);
        firstResult.StatusCode.ShouldBe(HttpStatusCode.Created);

        var secondResult = await _httpClient.PostAsJsonAsync(METHOD, request);
        secondResult.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var body = await secondResult.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.BILLING_ALREADY_EXISTS);
    }
}
