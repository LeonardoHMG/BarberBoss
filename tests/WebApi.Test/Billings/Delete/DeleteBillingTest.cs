using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Billings.Delete;

public class DeleteBillingTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Billings";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;
    private readonly string _otherBarberEmail;
    private readonly string _otherBarberPassword;

    public DeleteBillingTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
        _otherBarberEmail = webApplicationFactory.OtherBarber.GetEmail();
        _otherBarberPassword = webApplicationFactory.OtherBarber.GetPassword();
    }

    [Fact]
    public async Task Success_Barber_Can_Delete_Own_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("beard");

        var result = await _httpClient.DeleteAsync($"{METHOD}/{billingId}");

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResult = await _httpClient.GetAsync($"{METHOD}/{billingId}");
        getResult.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Success_Admin_Can_Delete_Any_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("hair");

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{billingId}");

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Error_Barber_Cannot_Delete_Other_Barber_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("hair and eyebrow");

        await _httpClient.AuthenticateAsync(_otherBarberEmail, _otherBarberPassword);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{billingId}");

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.BILLING_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Billing_Not_Found()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.BILLING_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var result = await _httpClient.DeleteAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }
}
