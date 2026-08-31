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

    //[Fact]
    //public async Task Success_Register_Reuses_Email_From_Deactivated_User()
    //{
    //    await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

    //    var newUserRequest = RequestRegisterUserJsonBuilder.Build();
    //    var registerResult = await _httpClient.PostAsJsonAsync("api/Users", newUserRequest);

    //    var registerBody = await registerResult.Content.ReadAsStreamAsync();
    //    var registerResponse = await JsonDocument.ParseAsync(registerBody);

    //    var newUserId = registerResponse.RootElement.GetProperty("id").GetGuid();

    //    await _httpClient.AuthenticateAsync(newUserRequest.Email, newUserRequest.Password);
    //    await _httpClient.RegisterBillingAsync("force deactivation");

    //    await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

    //    var deleteResult = await _httpClient.DeleteAsync($"api/Users/{newUserId}");
    //    deleteResult.StatusCode.ShouldBe(HttpStatusCode.NoContent);

    //    var loginResult = await _httpClient.PostAsJsonAsync("api/Login", new RequestLoginJson
    //    {
    //        Email = newUserRequest.Email,
    //        Password = newUserRequest.Password
    //    });

    //    loginResult.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

    //    var reusedEmailRequest = RequestRegisterUserJsonBuilder.Build();
    //    reusedEmailRequest.Email = newUserRequest.Email;

    //    var newRegisterResult = await _httpClient.PostAsJsonAsync("api/Users", reusedEmailRequest);
    //    newRegisterResult.StatusCode.ShouldBe(HttpStatusCode.Created);
    //}

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
