using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Jop.Services.Implemntations
{
   public class ZoomTokenService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public ZoomTokenService(IConfiguration config, IHttpClientFactory factory)
    {
        _config = config;
        _http = factory.CreateClient();
    }

    public async Task<string> GetTokenAsync()
{
    var client = _http;
    var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");
    var clientId = _config["Zoom:ClientId"];
    var clientSecret = _config["Zoom:ClientSecret"];
    var yourEncodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", yourEncodedCredentials);
    var accountId = _config["Zoom:AccountId"];
    var requestBody = $"grant_type=account_credentials&account_id={accountId}";
    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/x-www-form-urlencoded");

    var response = await client.SendAsync(request);

    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine("Zoom token response: " + content); // TEMPORARY: remove in production

    using var document = JsonDocument.Parse(content);
    var root = document.RootElement;

    if (root.TryGetProperty("access_token", out var token))
    {
        return token.GetString();
    }

    throw new Exception("access_token not found in Zoom response: " + content);
}

}
}   