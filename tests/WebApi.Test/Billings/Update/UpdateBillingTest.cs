using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Billings.Update;
public class UpdateBillingTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Billings";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;
    private readonly string _otherBarberEmail;
    private readonly string _otherBarberPassword;

    public UpdateBillingTest(CustomWebApplicationFactory webApplicationFactory)
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
    public async Task Success_Barber_Can_Update_Own_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("hair");

        var updateRequest = RequestBillingJsonBuilder.Build("Kids' haircut");

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{billingId}", updateRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResult = await _httpClient.GetAsync($"{METHOD}/{billingId}");

        var getBody = await getResult.Content.ReadAsStreamAsync();

        var getResponse = await JsonDocument.ParseAsync(getBody);

        getResponse.RootElement.GetProperty("serviceName").GetString().ShouldBe("Kids' haircut");
    }

    [Fact]
    public async Task Success_Admin_Can_Update_Any_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("beard");

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var updateRequest = RequestBillingJsonBuilder.Build("hair and beard");

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{billingId}", updateRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Error_Barber_Cannot_Update_Other_Barber_Billing()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("hair and eyebrow");

        await _httpClient.AuthenticateAsync(_otherBarberEmail, _otherBarberPassword);

        var updateRequest = RequestBillingJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{billingId}", updateRequest);

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

        var updateRequest = RequestBillingJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{Guid.NewGuid()}", updateRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.BILLING_NOT_FOUND);
    }

    [Fact]
    public async Task Error_ServiceName_Empty()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var billingId = await _httpClient.RegisterBillingAsync("hair");

        var updateRequest = RequestBillingJsonBuilder.Build();
        updateRequest.ServiceName = string.Empty;

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{billingId}", updateRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.SERVICE_NAME_REQUIRED);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var updateRequest = RequestBillingJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{Guid.NewGuid()}", updateRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }
}
