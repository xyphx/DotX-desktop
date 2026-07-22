using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotX.Desktop.Models;

namespace DotX.Desktop.ViewModels.Components;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<NavItemModel> _navItems;

    public SidebarViewModel()
    {
        _navItems = new ObservableCollection<NavItemModel>
        {
            new NavItemModel { Title = "Dashboard", Icon = "ViewDashboard", IsActive = true },
            new NavItemModel { Title = "Projects", Icon = "Folder" },
            new NavItemModel { Title = "AI Agents", Icon = "Robot" },
            new NavItemModel { Title = "Templates", Icon = "CardBulleted" },
            new NavItemModel { Title = "Knowledge Base", Icon = "BookOpen" },
            new NavItemModel { Title = "Integrations", Icon = "Connection" },
            new NavItemModel { Title = "History", Icon = "History" },
            new NavItemModel { Title = "Settings", Icon = "Cog" }
        };
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
}
