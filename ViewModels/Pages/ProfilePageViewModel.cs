using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Models;
using DotX.Desktop.Messages;
using DotX.Desktop.Services;

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
    public void Logout()
    {
        WeakReferenceMessenger.Default.Send(new LogoutMessage());
    }
}
