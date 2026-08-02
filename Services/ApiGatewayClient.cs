using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotX.Desktop.Services;

public class ApiGatewayClient
{
    private static ApiGatewayClient? _instance;
    public static ApiGatewayClient Instance => _instance ??= new ApiGatewayClient();

    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private ApiGatewayClient()
    {
        _httpClient = new HttpClient();
        _baseUrl = Environment.GetEnvironmentVariable("DOTX_API_URL") ?? "https://api.dotx.xyphx.com";
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, object? data = null)
    {
        return await SendRequestAsync(HttpMethod.Post, endpoint, data);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return await SendRequestAsync(HttpMethod.Get, endpoint);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string endpoint, object? data = null, bool isRetry = false)
    {
        var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}");

        var token = UserSession.Instance.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (data != null)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        var response = await _httpClient.SendAsync(request);

        // Handle 401 Unauthorized globally
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !isRetry && !endpoint.Contains("/auth/"))
        {
            var refreshed = await RefreshTokensAsync();
            if (refreshed)
            {
                // Retry original request
                return await SendRequestAsync(method, endpoint, data, true);
            }
            else
            {
                // Refresh failed, logout
                UserSession.Instance.Logout();
            }
        }

        return response;
    }

    private async Task<bool> RefreshTokensAsync()
    {
        var refreshToken = UserSession.Instance.RefreshToken;
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/auth/refresh");
        var payload = JsonSerializer.Serialize(new { refreshToken }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var newAccessToken = doc.RootElement.GetProperty("accessToken").GetString();
            var newRefreshToken = doc.RootElement.GetProperty("refreshToken").GetString();

            if (!string.IsNullOrEmpty(newAccessToken) && !string.IsNullOrEmpty(newRefreshToken))
            {
                UserSession.Instance.SetTokens(newAccessToken, newRefreshToken);
                return true;
            }
        }
        
        return false;
    }
}
