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
                ErrorMessage = string.IsNullOrWhiteSpace(err) ? "Invalid API Key." : err;
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
                ErrorMessage = "Failed to parse user data.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    [RelayCommand]
    public void OpenConsole()
    {
        var url = "https://xyphx.com/";
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
