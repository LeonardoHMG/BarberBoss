using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.UpdateById;
public class UpdateUserByIdTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;
    private readonly string _otherBarberEmail;

    public UpdateUserByIdTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _otherBarberEmail = webApplicationFactory.OtherBarber.GetEmail();
    }

    [Fact]
    public async Task Success()
    {
        var userId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{userId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResult = await _httpClient.GetAsync($"{METHOD}/{userId}");

        var getBody = await getResult.Content.ReadAsStreamAsync();
        var getResponse = await JsonDocument.ParseAsync(getBody);

        getResponse.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        getResponse.RootElement.GetProperty("email").GetString().ShouldBe(request.Email);
        getResponse.RootElement.GetProperty("role").GetString().ShouldBe(request.Role.ToString());
        getResponse.RootElement.GetProperty("isActive").GetBoolean().ShouldBe(request.IsActive);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        var nonExistentUserId = Guid.NewGuid();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{nonExistentUserId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.USER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Email_Already_In_Use()
    {
        var userId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Email = _otherBarberEmail; 

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{userId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        var userId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        var request = RequestUpdateUserByAdminJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{userId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.NAME_EMPTY);
    }

    [Fact]
    public async Task Error_Non_Admin_User_Cannot_Access()
    {
        var userId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);

        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{userId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var userId = await _httpClient.RegisterUserAsync(_emailAdmin, _passwordAdmin);
        _httpClient.DefaultRequestHeaders.Authorization = null;

        var request = RequestUpdateUserByAdminJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync($"{METHOD}/{userId}", request);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }
}