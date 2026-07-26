using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.ViewModels.Components;

namespace DotX.Desktop.ViewModels.Pages;

public partial class DashboardHomeViewModel : ViewModelBase
{
    public DocumentUploadViewModel DocumentUpload { get; } = new();
    public AgentsListViewModel AgentsList { get; } = new();
}
