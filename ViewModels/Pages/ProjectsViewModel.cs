using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.Models;

namespace DotX.Desktop.ViewModels.Pages;

public partial class ProjectsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<ProjectModel> _projects;

    public ProjectsViewModel()
    {
        _projects = new ObservableCollection<ProjectModel>
        {
            new ProjectModel { Name = "E-commerce Platform", Description = "Microservices based shopping app with AI recommendations", LastModified = "2 hours ago", Status = "Active", Icon = "Cart" },
            new ProjectModel { Name = "Banking Portal", Description = "Secure fintech dashboard for customer banking", LastModified = "Yesterday", Status = "Review", Icon = "Bank" },
            new ProjectModel { Name = "Health Tracker", Description = "Fitness app syncing with wearables and AI coach", LastModified = "3 days ago", Status = "Draft", Icon = "HeartPulse" },
            new ProjectModel { Name = "CRM System", Description = "Internal tool for sales and lead management", LastModified = "Last week", Status = "Completed", Icon = "AccountGroup" }
        };
    }
}
