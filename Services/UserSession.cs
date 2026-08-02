using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotX.Desktop.Models;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Messages;

namespace DotX.Desktop.Services;

public class UserSession
{
    public static UserSession Instance { get; } = new();

    public UserModel? CurrentUser { get; set; }

    public string AccessToken { get; private set; } = string.Empty;
    
    public string RefreshToken { get; private set; } = string.Empty;

    public bool IsLoggedIn => CurrentUser != null && !string.IsNullOrEmpty(AccessToken);

    private readonly string _tokenFilePath;

    private UserSession()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dotxFolder = Path.Combine(appData, "DotX");
        Directory.CreateDirectory(dotxFolder);
        _tokenFilePath = Path.Combine(dotxFolder, "session.dat");

        LoadRefreshToken();
    }

    public void SetTokens(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        SaveRefreshToken();
    }

    public void Logout()
    {
        CurrentUser = null;
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
        
        if (File.Exists(_tokenFilePath))
        {
            File.Delete(_tokenFilePath);
        }

        // Send a message to navigate back to login
        WeakReferenceMessenger.Default.Send(new AppNavigationMessage("Login"));
    }

    private void SaveRefreshToken()
    {
        if (string.IsNullOrEmpty(RefreshToken)) return;

        try
        {
            var bytes = Encoding.UTF8.GetBytes(RefreshToken);
            // Protect data using Windows Data Protection API (DPAPI) if on Windows, else fallback to plain
            byte[] encrypted;
            if (OperatingSystem.IsWindows())
            {
                #pragma warning disable CA1416 // Validate platform compatibility
                encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                #pragma warning restore CA1416
            }
            else
            {
                encrypted = bytes; // Basic fallback for non-Windows (could use other vault APIs)
            }
            
            File.WriteAllBytes(_tokenFilePath, encrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save refresh token: {ex.Message}");
        }
    }

    private void LoadRefreshToken()
    {
        if (!File.Exists(_tokenFilePath)) return;

        try
        {
            var encrypted = File.ReadAllBytes(_tokenFilePath);
            byte[] decrypted;
            if (OperatingSystem.IsWindows())
            {
                #pragma warning disable CA1416
                decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                #pragma warning restore CA1416
            }
            else
            {
                decrypted = encrypted;
            }

            RefreshToken = Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load refresh token: {ex.Message}");
            // Corrupted or unable to decrypt, delete it
            File.Delete(_tokenFilePath);
        }
    }
}
