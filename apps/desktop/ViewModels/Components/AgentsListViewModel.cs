using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.Models;

namespace DotX.Desktop.ViewModels.Components;

public partial class AgentsListViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<AgentModel> _agents;

    public AgentsListViewModel()
    {
        _agents = new ObservableCollection<AgentModel>
        {
            new AgentModel { Name = "Security Analyst Agent", Description = "Analyzes security requirements and threats.", Icon = "ShieldAccount" },
            new AgentModel { Name = "UI/UX Design Agent", Description = "Creates wireframes, UI flows and design systems.", Icon = "Palette" },
            new AgentModel { Name = "Business Analyst Agent", Description = "Analyzes business logic and requirements.", Icon = "MapMarker" },
            new AgentModel { Name = "System Architect Agent", Description = "Designs system architecture and tech stack.", Icon = "TableLarge" },
            new AgentModel { Name = "Database Engineer Agent", Description = "Designs database schema and data models.", Icon = "Database" },
            new AgentModel { Name = "DevOps Engineer Agent", Description = "Handles CI/CD, deployment and infrastructure.", Icon = "CloudCog" }
        };
    }
}
