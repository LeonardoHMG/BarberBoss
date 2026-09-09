using BarberBoss.Exception;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.Profile;
public class GetUserProfileTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public GetUserProfileTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Barber_Can_Get_Own_Profile()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.GetAsync(METHOD);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("id").GetGuid().ShouldNotBe(Guid.Empty);
        response.RootElement.GetProperty("name").GetString().ShouldNotBeNullOrWhiteSpace();
        response.RootElement.GetProperty("email").GetString().ShouldBe(_emailBarber);
        response.RootElement.GetProperty("role").GetString().ShouldBe("barber");
    }

    [Fact]
    public async Task Success_Admin_Can_Get_Own_Profile()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.GetAsync(METHOD);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("id").GetGuid().ShouldNotBe(Guid.Empty);
        response.RootElement.GetProperty("name").GetString().ShouldNotBeNullOrWhiteSpace();
        response.RootElement.GetProperty("email").GetString().ShouldBe(_emailAdmin);
        response.RootElement.GetProperty("role").GetString().ShouldBe("administrator");
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

}
