using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.Register;
public class RegisterUserTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public RegisterUserTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
    }

    [Fact]
    public async Task Success_Admin_Can_Register()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);

        response.RootElement.GetProperty("token").GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Error_Barber_Cannot_Register()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        
        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.FORBIDDEN);

    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestRegisterUserJsonBuilder.Build();

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }

    [Fact]
    public async Task Error_Empty_Name()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.NAME_EMPTY);
    }
}
