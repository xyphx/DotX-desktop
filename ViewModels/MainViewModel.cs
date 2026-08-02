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
    }

    private void OnLoginSuccess(UserModel user)
    {
        // Switch to the Dashboard with user profile data
        CurrentPage = new DashboardViewModel(user);
    }
}
