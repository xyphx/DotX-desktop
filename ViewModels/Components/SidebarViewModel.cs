using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotX.Desktop.Models;
using DotX.Desktop.Services;
using DotX.Desktop.Messages;
using CommunityToolkit.Mvvm.Messaging;

namespace DotX.Desktop.ViewModels.Components;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserModel? _currentUser;

    [ObservableProperty]
    private ObservableCollection<NavItemModel> _navItems;

    [ObservableProperty]
    private NavItemModel? _selectedNavItem;

    partial void OnSelectedNavItemChanged(NavItemModel? value)
    {
        if (value == null) return;
        foreach (var item in NavItems)
        {
            item.IsActive = item == value;
        }
    }

    public SidebarViewModel(UserModel? user = null)
    {
        CurrentUser = user ?? UserSession.Instance.CurrentUser;

        _navItems = new ObservableCollection<NavItemModel>
        {
            new NavItemModel { Title = "Dashboard", Icon = "ViewDashboard" },
            new NavItemModel { Title = "Projects", Icon = "Folder" },
            new NavItemModel { Title = "AI Agents", Icon = "Robot" },
            new NavItemModel { Title = "History", Icon = "History" },
            new NavItemModel { Title = "Settings", Icon = "Cog" }
        };
        SelectedNavItem = _navItems[0];
    }

    [RelayCommand]
    public void NewProject()
    {
        Console.WriteLine("New Project Requested");
    }

    [RelayCommand]
    public void Upgrade()
    {
        Console.WriteLine("Upgrade Requested");
    }

    [RelayCommand]
    public void OpenProfile()
    {
        SelectedNavItem = null;
        CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send(new AppNavigationMessage("Profile"));
    }
}
