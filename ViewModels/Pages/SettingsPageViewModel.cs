using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace DotX.Desktop.ViewModels.Pages;

public partial class SettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isDarkModeEnabled = true;

    [ObservableProperty]
    private bool _isAnalyticsEnabled = false;

    [ObservableProperty]
    private string _selectedModel = "GPT-4";

    [RelayCommand]
    public void DeleteProject()
    {
        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(
            new DotX.Desktop.Messages.AppNotificationMessage("Project deletion initiated (mock)."));
    }
}
