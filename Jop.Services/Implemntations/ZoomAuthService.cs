using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Jop.Services.Implemntations
{
   public class ZoomOAuthService
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpFactory;

    public ZoomOAuthService(IConfiguration config, IHttpClientFactory factory)
    {
        _config = config;
        _httpFactory = factory;
    }

    public string GetAuthUrl()
    {
        var clientId = _config["Zoom:ClientId"];
        var redirectUri = _config["Zoom:RedirectUri"];
        return $"https://zoom.us/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}";
    }

    public async Task<string> ExchangeCodeForTokenAsync(string code)
    {
        var client = _httpFactory.CreateClient();

        var clientId = _config["Zoom:ClientId"];
        var clientSecret = _config["Zoom:ClientSecret"];
        var redirectUri = _config["Zoom:RedirectUri"];

        var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");
        var parameters = new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"code", code},
            {"redirect_uri", redirectUri}
        };

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")));

        request.Content = new FormUrlEncodedContent(parameters);

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"OAuth error: {content}");

        return content; // contains access_token, refresh_token, etc.
    }
}

}