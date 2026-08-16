using BarberBoss.Communication.Requests;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Utils;
public static class HttpClientAuthExtensions
{
    public static async Task AuthenticateAsync(this HttpClient httpClient, string email, string password)
    {
        var loginRequest = new RequestLoginJson
        {
            Email = email,
            Password = password
        };

        var result = await httpClient.PostAsJsonAsync("api/Login", loginRequest);

        var body = await result.Content.ReadAsStreamAsync();
        var response = await JsonDocument.ParseAsync(body);

        if (!result.IsSuccessStatusCode)
        {
            var raw = response.RootElement.GetRawText();
            throw new Exception($"Login failed with status {result.StatusCode}: {raw}");
        }

        var token = response.RootElement.GetProperty("token").GetString();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
