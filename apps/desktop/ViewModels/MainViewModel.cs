using CommunityToolkit.Mvvm.ComponentModel;

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

    private void OnLoginSuccess()
    {
        // Switch to the Dashboard
        CurrentPage = new DashboardViewModel();
    }
}
