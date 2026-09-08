using BarberBoss.Exception;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.GetById;

public class GetUserByIdTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public GetUserByIdTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Admin_Can_Get_User_By_Id()
    {
        var createdUserId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.GetAsync($"{METHOD}/{createdUserId}");

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        response.RootElement.GetProperty("id").GetGuid().ShouldBe(createdUserId);
    }

    [Fact]
    public async Task Error_Barber_Cannot_Get_User_By_Id()
    {
        var createdUserId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.GetAsync($"{METHOD}/{createdUserId}");

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.FORBIDDEN);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.GetAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.USER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var result = await _httpClient.GetAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();
        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }
}
