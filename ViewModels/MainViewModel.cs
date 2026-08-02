using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.Models;

namespace DotX.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainViewModel()
    {
        // Start at the Login Screen
        _currentPage = new LoginViewModel(OnLoginSuccess);

        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Register<DotX.Desktop.Messages.LogoutMessage>(this, (r, m) =>
        {
            DotX.Desktop.Services.UserSession.Instance.CurrentUser = null;
            CurrentPage = new LoginViewModel(OnLoginSuccess);
        });
    }

    private void OnLoginSuccess(UserModel user)
    {
        // Switch to the Dashboard with user profile data
        CurrentPage = new DashboardViewModel(user);
    }
}
