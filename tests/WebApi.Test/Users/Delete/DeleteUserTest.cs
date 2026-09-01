using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.Delete;
public class DeleteUserTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;
    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public DeleteUserTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
    }

    [Fact]
    public async Task Success_Hard_Deletes_User_Without_Billings()
    {
        var userId = await CreateUserAndGetId();

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{userId}");

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Success_Deactivates_User_With_Billings()
    {
        var (userId, email, password) = await CreateUserWithBillingAndGetCredentials();

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{userId}");

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var loginResult = await _httpClient.PostAsJsonAsync("api/Login", new
        {
            Email = email,
            Password = password
        });

        loginResult.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Cannot_Delete_Own_Account()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var profileResult = await _httpClient.GetAsync("api/Users");
        var profileBody = await profileResult.Content.ReadAsStreamAsync();
        var profileResponse = await JsonDocument.ParseAsync(profileBody);
        var adminId = profileResponse.RootElement.GetProperty("id").GetGuid();

        var result = await _httpClient.DeleteAsync($"{METHOD}/{adminId}");

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.CANNOT_DELETE_OWN_ACCOUNT);
    }

    [Fact]
    public async Task Error_User_Not_Found()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.USER_NOT_FOUND);
    }

    [Fact]
    public async Task Error_Barber_Cannot_Delete_User()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var result = await _httpClient.DeleteAsync($"{METHOD}/{Guid.NewGuid()}");

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.FORBIDDEN);
    }

    [Fact]
    public async Task Success_Register_Reuses_Email_From_Deactivated_User()
    {
        var (userId, email, password) = await CreateUserWithBillingAndGetCredentials();

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var deleteResult = await _httpClient.DeleteAsync($"{METHOD}/{userId}");
        deleteResult.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var loginResult = await _httpClient.PostAsJsonAsync("api/Login", new { Email = email, Password = password });
        loginResult.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var reusedEmailRequest = RequestRegisterUserJsonBuilder.Build();
        reusedEmailRequest.Email = email;

        var newRegisterResult = await _httpClient.PostAsJsonAsync(METHOD, reusedEmailRequest);
        newRegisterResult.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    private async Task<Guid> CreateUserAndGetId()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestRegisterUserJsonBuilder.Build();
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        return response.RootElement.GetProperty("id").GetGuid();
    }

    private async Task<(Guid Id, string Email, string Password)> CreateUserWithBillingAndGetCredentials()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestRegisterUserJsonBuilder.Build();
        var result = await _httpClient.PostAsJsonAsync(METHOD, request);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);
        var userId = response.RootElement.GetProperty("id").GetGuid();

        await _httpClient.AuthenticateAsync(request.Email, request.Password);
        await _httpClient.RegisterBillingAsync("Kids' haircut");

        return (userId, request.Email, request.Password);
    }
}