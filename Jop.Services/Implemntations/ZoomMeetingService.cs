using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Jop.Services.Implemntations
{
   public class ZoomMeetingService
{
    private readonly ZoomTokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public ZoomMeetingService(ZoomTokenService tokenService, IConfiguration config, IHttpClientFactory factory)
    {
        _tokenService = tokenService;
        _config = config;
        _http = factory.CreateClient();
    }

    public async Task<string> CreateMeetingAsync(DateTime time, string topic, int durationn)
    {
        var token = await _tokenService.GetTokenAsync();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            topic,
            type = 2,
            start_time = time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            duration = durationn,
            timezone = "UTC",
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        var res = await _http.PostAsync($"{_config["Zoom:BaseUrl"]}/users/me/meetings", content);

        var json = await res.Content.ReadAsStringAsync();
        var obj = JsonDocument.Parse(json);
        return obj.RootElement.GetProperty("join_url").GetString();
    }
}

}