using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Models;
using DotX.Desktop.Messages;
using DotX.Desktop.Services;
using System.Threading.Tasks;

namespace DotX.Desktop.ViewModels.Pages;

public partial class ProfilePageViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserModel _currentUser;

    public ProfilePageViewModel()
    {
        _currentUser = UserSession.Instance.CurrentUser ?? new UserModel();
    }

    [RelayCommand]
    public async Task LogoutAsync()
    {
        var refreshToken = UserSession.Instance.RefreshToken;
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var payload = new { refreshToken = refreshToken };
            try
            {
                await ApiGatewayClient.Instance.PostAsync("/api/auth/logout", payload);
            }
            catch
            {
                // Ignore errors on logout
            }
        }
        
        UserSession.Instance.Logout();
        WeakReferenceMessenger.Default.Send(new LogoutMessage());
    }
}
