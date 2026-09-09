using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.ChangePassword;
public class ChangePasswordTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users/change-password";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;

    public ChangePasswordTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Changes_Password_And_Allows_New_Login()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var newUserRequest = RequestRegisterUserJsonBuilder.Build();
        await _httpClient.PostAsJsonAsync("api/Users", newUserRequest);

        await _httpClient.AuthenticateAsync(newUserRequest.Email, newUserRequest.Password);

        var newPassword = "!Aa1BrandNewPass123";
        var changePasswordRequest = new RequestChangePasswordJson
        {
            Password = newUserRequest.Password,
            NewPassword = newPassword
        };

        var result = await _httpClient.PutAsJsonAsync(METHOD, changePasswordRequest);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var oldLoginResult = await _httpClient.PostAsJsonAsync("api/Login", new RequestLoginJson
        {
            Email = newUserRequest.Email,
            Password = newUserRequest.Password
        });
        oldLoginResult.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var newLoginResult = await _httpClient.PostAsJsonAsync("api/Login", new RequestLoginJson
        {
            Email = newUserRequest.Email,
            Password = newPassword
        });
        newLoginResult.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Error_Current_Password_Invalid()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var newUserRequest = RequestRegisterUserJsonBuilder.Build();
        await _httpClient.PostAsJsonAsync("api/Users", newUserRequest);

        await _httpClient.AuthenticateAsync(newUserRequest.Email, newUserRequest.Password);

        var request = new RequestChangePasswordJson
        {
            Password = "WrongCurrentPassword123!",
            NewPassword = "!Aa1NewPassword123"
        };

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.PASSWORD_DIFFERENT_CURRENT_PASSWORD);
    }

    [Fact]
    public async Task Error_New_Password_Invalid()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var newUserRequest = RequestRegisterUserJsonBuilder.Build();
        await _httpClient.PostAsJsonAsync("api/Users", newUserRequest);

        await _httpClient.AuthenticateAsync(newUserRequest.Email, newUserRequest.Password);

        var request = new RequestChangePasswordJson
        {
            Password = newUserRequest.Password,
            NewPassword = "weak"
        };

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.INVALID_PASSWORD);
    }

    [Fact]
    public async Task Error_Password_Empty()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var newUserRequest = RequestRegisterUserJsonBuilder.Build();
        await _httpClient.PostAsJsonAsync("api/Users", newUserRequest);

        await _httpClient.AuthenticateAsync(newUserRequest.Email, newUserRequest.Password);

        var request = new RequestChangePasswordJson
        {
            Password = string.Empty,
            NewPassword = "!Aa1NewPassword123"
        };

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.PASSWORD_REQUIRED);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson
        {
            Password = "AnyPassword123!",
            NewPassword = "!Aa1NewPassword123"
        };

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }
}