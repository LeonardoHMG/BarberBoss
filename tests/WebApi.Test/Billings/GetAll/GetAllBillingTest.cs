using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Billings.GetAll;

public class GetAllBillingTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Billings";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public GetAllBillingTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Barber_Can_List_Own_Billings()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var registerRequest = RequestRegisterBillingJsonBuilder.Build("hair");
        await _httpClient.PostAsJsonAsync(METHOD, registerRequest);

        var result = await _httpClient.GetAsync(METHOD);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var billings = response.RootElement.GetProperty("billings").EnumerateArray();

        billings.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Success_Admin_Can_List_All_Billings()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var registerRequest = RequestRegisterBillingJsonBuilder.Build("hair and eyebrow");
        await _httpClient.PostAsJsonAsync(METHOD, registerRequest);

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.GetAsync(METHOD);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("totalCount").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task Success_Filter_By_ServiceName()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var uniqueServiceName = "Special Test Cut";
        var registerRequest = RequestRegisterBillingJsonBuilder.Build(uniqueServiceName);
        await _httpClient.PostAsJsonAsync("api/Billings", registerRequest);

        var result = await _httpClient.GetAsync($"{METHOD}?serviceName={Uri.EscapeDataString(uniqueServiceName)}");

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var billings = response.RootElement.GetProperty("billings").EnumerateArray().ToList();

        billings.ShouldNotBeEmpty();
        billings.ShouldAllBe(b => b.GetProperty("serviceName").GetString() == uniqueServiceName);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var result = await _httpClient.GetAsync(METHOD);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }

    [Fact]
    public async Task Error_PageNumber_Invalid()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.GetAsync($"{METHOD}?pageNumber=0");

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.PAGE_NUMBER_INVALID);
    }

    [Fact]
    public async Task Error_MinAmount_Greater_Than_MaxAmount()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.GetAsync($"{METHOD}?minAmount=500&maxAmount=100");

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.MIN_AMOUNT_GREATER_THAN_MAX_AMOUNT);
    }
}
