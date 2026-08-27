using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Users.Update;
public class UpdateUserTest: IClassFixture<CustomWebApplicationFactory>
{
    private const string METHOD = "api/Users";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;
    private readonly string _otherBarberEmail;
    private readonly string _otherBarberPassword;

    public UpdateUserTest(CustomWebApplicationFactory webApplicationFactory)
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
    public async Task Success_Barber_Can_Update_Own_Profile()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestUpdateUserJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResult = await _httpClient.GetAsync(METHOD);

        var getBody = await getResult.Content.ReadAsStreamAsync();
        var getResponse = await JsonDocument.ParseAsync(getBody);

        getResponse.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        getResponse.RootElement.GetProperty("email").GetString().ShouldBe(request.Email);
    }

    [Fact]
    public async Task Success_Admin_Can_Update_Own_Profile()
    {
        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var request = RequestUpdateUserJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Error_Email_Already_In_Use()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = _otherBarberEmail;

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
    }

    [Fact]
    public async Task Error_Name_Empty()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.NAME_EMPTY);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestUpdateUserJsonBuilder.Build();

        var result = await _httpClient.PutAsJsonAsync(METHOD, request);

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }

}
