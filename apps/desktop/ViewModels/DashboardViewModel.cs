using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Messages;
using DotX.Desktop.ViewModels.Components;
using DotX.Desktop.ViewModels.Pages;

namespace DotX.Desktop.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public SidebarViewModel Sidebar { get; } = new();

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private string _notificationMessage = string.Empty;

    [ObservableProperty]
    private bool _isNotificationVisible;

    public DashboardViewModel()
    {
        // Default page
        _currentPage = new DashboardHomeViewModel();

        Sidebar.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Sidebar.SelectedNavItem))
            {
                NavigateTo(Sidebar.SelectedNavItem?.Title);
            }
        };

        WeakReferenceMessenger.Default.Register<AppNotificationMessage>(this, (r, m) =>
        {
            NotificationMessage = m.Value;
            IsNotificationVisible = true;

            // Hide after 3 seconds
            System.Threading.Tasks.Task.Delay(3000).ContinueWith(_ =>
            {
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IsNotificationVisible = false;
                });
            });
        });
    }

    private void NavigateTo(string? title)
    {
        if (title == "Projects")
        {
            CurrentPage = new ProjectsViewModel();
        }
        else if (title == "Dashboard")
        {
            CurrentPage = new DashboardHomeViewModel();
        }
        else if (title == "AI Agents")
        {
            CurrentPage = new AgentsPageViewModel();
        }
        else if (title == "History")
        {
            CurrentPage = new HistoryPageViewModel();
        }
        else if (title == "Settings")
        {
            CurrentPage = new SettingsPageViewModel();
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"{title} is not implemented yet."));
        }
    }

    [RelayCommand]
    public void ViewProjects()
    {
        // For buttons that navigate to Projects view explicitly
        CurrentPage = new ProjectsViewModel();
    }
}
