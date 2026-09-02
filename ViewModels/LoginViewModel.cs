using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotX.Desktop.Models;
using DotX.Desktop.Services;

namespace DotX.Desktop.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly Action<UserModel> _onLoginSuccess;

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoggingIn;

    public LoginViewModel(Action<UserModel> onLoginSuccess)
    {
        _onLoginSuccess = onLoginSuccess;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            ErrorMessage = "Please enter your API key.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length != 6)
        {
            ErrorMessage = "Please enter a valid 6-digit PIN.";
            return;
        }

        IsLoggingIn = true;
        ErrorMessage = string.Empty;

        try
        {
            var payload = new 
            { 
                apiKey = ApiKey.Trim(), 
                password = Password.Trim(),
                deviceId = Guid.NewGuid().ToString(),
                deviceName = Environment.MachineName,
                platform = RuntimeInformation.OSDescription
            };

            var response = await ApiGatewayClient.Instance.PostAsync("/api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                ErrorMessage = FormatErrorMessage(err);
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            
            var root = doc.RootElement;
            var accessToken = root.GetProperty("accessToken").GetString();
            var refreshToken = root.GetProperty("refreshToken").GetString();
            
            var userElement = root.GetProperty("user");
            var user = JsonSerializer.Deserialize<UserModel>(userElement.GetRawText(), new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (user != null && !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                UserSession.Instance.SetTokens(accessToken, refreshToken);
                UserSession.Instance.CurrentUser = user;
                _onLoginSuccess?.Invoke(user);
            }
            else
            {
                ErrorMessage = "Failed to parse user session.";
            }
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to connect to server. Please check your internet connection.";
        }
        catch (Exception ex)
        {
            ErrorMessage = FormatErrorMessage(ex.Message);
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    private static string FormatErrorMessage(string? rawError)
    {
        if (string.IsNullOrWhiteSpace(rawError))
            return "Invalid API Key or Security PIN.";

        var clean = rawError.Trim();

        // Trim quotation marks if the server returned a quoted JSON string
        if (clean.StartsWith("\"") && clean.EndsWith("\"") && clean.Length >= 2)
        {
            clean = clean.Substring(1, clean.Length - 2);
        }

        // Extract detail from gRPC status string e.g. Status(StatusCode="NotFound", Detail="Invalid API Key")
        if (clean.Contains("Detail=", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(clean, @"Detail=""([^""]+)""", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
            {
                clean = match.Groups[1].Value;
            }
        }

        // If JSON error payload like { "message": "..." } or { "error": "..." }
        if (clean.StartsWith("{") && clean.EndsWith("}"))
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(clean);
                if (jsonDoc.RootElement.TryGetProperty("message", out var msgProp) && !string.IsNullOrWhiteSpace(msgProp.GetString()))
                {
                    clean = msgProp.GetString()!;
                }
                else if (jsonDoc.RootElement.TryGetProperty("error", out var errProp) && !string.IsNullOrWhiteSpace(errProp.GetString()))
                {
                    clean = errProp.GetString()!;
                }
                else if (jsonDoc.RootElement.TryGetProperty("detail", out var detailProp) && !string.IsNullOrWhiteSpace(detailProp.GetString()))
                {
                    clean = detailProp.GetString()!;
                }
            }
            catch
            {
                // Fall back to current string
            }
        }

        // Map known error patterns to clear, friendly user messages
        if (clean.Contains("Invalid API Key", StringComparison.OrdinalIgnoreCase) || clean.Contains("NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid API Key. Please verify your key in the console.";
        }
        
        if (clean.Contains("Invalid PIN", StringComparison.OrdinalIgnoreCase) || clean.Contains("Invalid Password", StringComparison.OrdinalIgnoreCase) || clean.Contains("PIN", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid Security PIN. Please enter your 6-digit PIN.";
        }

        if (clean.Contains("Unauthenticated", StringComparison.OrdinalIgnoreCase) || clean.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid API Key or Security PIN.";
        }

        return clean;
    }

    [RelayCommand]
    public void OpenConsole()
    {
        var url = "https://xyphx.com/console";
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not open browser: {ex.Message}");
        }
    }
}