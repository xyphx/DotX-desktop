using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.ViewModels.Components;

namespace DotX.Desktop.ViewModels.Pages;

public partial class AgentsPageViewModel : ViewModelBase
{
    public AgentsListViewModel AgentsList { get; } = new();

    public AgentsPageViewModel()
    {
        AgentsList.IsViewAllButtonVisible = false;
    }
}
