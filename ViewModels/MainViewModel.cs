using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Models;
using DotX.Desktop.Messages;
using DotX.Desktop.Services;

namespace DotX.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    public MainViewModel()
    {
        // Start at the Login Screen
        _currentPage = new LoginViewModel(OnLoginSuccess);

        WeakReferenceMessenger.Default.Register<LogoutMessage>(this, (r, m) =>
        {
            UserSession.Instance.CurrentUser = null;
            CurrentPage = new LoginViewModel(OnLoginSuccess);
        });
    }

    private void OnLoginSuccess(UserModel user)
    {
        // Switch to the Dashboard with user profile data
        CurrentPage = new DashboardViewModel(user);
    }
}
