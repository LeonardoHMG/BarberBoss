using BarberBoss.Exception;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Test.Utils;

namespace WebApi.Test.Billings.Reports;
public class GenerateBillingsReportTest : IClassFixture<CustomWebApplicationFactory>
{
    private const string EXCEL_METHOD = "api/Report/excel";
    private const string PDF_METHOD = "api/Report/pdf";

    private readonly HttpClient _httpClient;

    private readonly string _emailAdmin;
    private readonly string _passwordAdmin;
    private readonly string _emailBarber;
    private readonly string _passwordBarber;

    public GenerateBillingsReportTest(CustomWebApplicationFactory webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
        _emailBarber = webApplicationFactory.Barber.GetEmail();
        _passwordBarber = webApplicationFactory.Barber.GetPassword();
        _emailAdmin = webApplicationFactory.Admin.GetEmail();
        _passwordAdmin = webApplicationFactory.Admin.GetPassword();
    }

    [Fact]
    public async Task Success_Admin_Can_Generate_Excel_Report()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        await _httpClient.RegisterBillingAsync("hair", DateTime.Now.AddMinutes(-5));

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _httpClient.GetAsync($"{EXCEL_METHOD}?date={today:yyyy-MM-dd}");

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bytes = await result.Content.ReadAsByteArrayAsync();
        bytes.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Success_Admin_Can_Generate_Pdf_Report()
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        await _httpClient.RegisterBillingAsync("hair", DateTime.Now.AddMinutes(-5));

        await _httpClient.AuthenticateAsync(_emailAdmin, _passwordAdmin);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _httpClient.GetAsync($"{PDF_METHOD}?date={today:yyyy-MM-dd}");

        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bytes = await result.Content.ReadAsByteArrayAsync();
        bytes.ShouldNotBeEmpty();
    }

    [Theory]
    [InlineData(EXCEL_METHOD)]
    [InlineData(PDF_METHOD)]
    public async Task Error_Barber_Cannot_Generate_Report(string method)
    {
        await _httpClient.AuthenticateAsync(_emailBarber, _passwordBarber);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _httpClient.GetAsync($"{method}?date={today:yyyy-MM-dd}");

        result.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.FORBIDDEN);
    }

    [Theory]
    [InlineData(EXCEL_METHOD)]
    [InlineData(PDF_METHOD)]
    public async Task Error_Without_Token(string method)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _httpClient.GetAsync($"{method}?date={today:yyyy-MM-dd}");

        result.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var body = await result.Content.ReadAsStreamAsync();

        var response = await JsonDocument.ParseAsync(body);

        var errors = response.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.ShouldHaveSingleItem().GetString()!.ShouldBe(ResourceErrorMessages.UNAUTHORIZED);
    }

}
