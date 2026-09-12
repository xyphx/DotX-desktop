using System;
using System.IO;
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
        var url = Environment.GetEnvironmentVariable("DOTX_API_URL");
        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("[ERROR] Missing required environment variable 'DOTX_API_URL'.");
            throw new InvalidOperationException("Missing required environment variable 'DOTX_API_URL'.");
        }
        _baseUrl = url.TrimEnd('/');
    }

    public async Task<HttpResponseMessage> PostAsync(string endpoint, object? data = null)
    {
        return await SendRequestAsync(HttpMethod.Post, endpoint, data);
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        return await SendRequestAsync(HttpMethod.Get, endpoint);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        return await SendRequestAsync(HttpMethod.Delete, endpoint);
    }

    public async Task<HttpResponseMessage> UploadFileAsync(string endpoint, Stream fileStream, string fileName, string? projectId = null, string? userId = null, bool isRetry = false)
    {
        var content = new MultipartFormDataContent();
        
        var streamContent = new StreamContent(fileStream);
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var mime = ext switch
        {
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".doc" => "application/msword",
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(mime);
        content.Add(streamContent, "file", fileName);

        var uid = userId;
        if (string.IsNullOrWhiteSpace(uid))
        {
            uid = UserSession.Instance.CurrentUser?.Id;
        }
        if (!string.IsNullOrWhiteSpace(uid))
        {
            content.Add(new StringContent(uid), "userId");
        }

        if (!string.IsNullOrWhiteSpace(projectId))
        {
            content.Add(new StringContent(projectId), "projectId");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{endpoint}")
        {
            Content = content
        };

        var token = UserSession.Instance.AccessToken;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.SendAsync(request);

        // Handle 401 Unauthorized globally
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && !isRetry && !endpoint.Contains("/auth/"))
        {
            var refreshed = await RefreshTokensAsync();
            if (refreshed)
            {
                if (fileStream.CanSeek)
                {
                    fileStream.Position = 0;
                    return await UploadFileAsync(endpoint, fileStream, fileName, projectId, userId, true);
                }
            }
            else
            {
                UserSession.Instance.Logout();
            }
        }

        return response;
    }

    public async Task<HttpResponseMessage> UploadFileAsync(string endpoint, string filePath, string? projectId = null, string? userId = null)
    {
        using var stream = File.OpenRead(filePath);
        var fileName = Path.GetFileName(filePath);
        return await UploadFileAsync(endpoint, stream, fileName, projectId, userId);
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
