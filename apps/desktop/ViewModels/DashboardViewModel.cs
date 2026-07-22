using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.ViewModels.Components;

namespace DotX.Desktop.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    public SidebarViewModel Sidebar { get; } = new();
    public DocumentUploadViewModel DocumentUpload { get; } = new();
    public AgentsListViewModel AgentsList { get; } = new();

    public DashboardViewModel()
    {
    }
}
