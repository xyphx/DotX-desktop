using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.Models;
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

    [CommunityToolkit.Mvvm.Input.RelayCommand]
    public void Logout()
    {
        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new DotX.Desktop.Messages.LogoutMessage());
    }
}
