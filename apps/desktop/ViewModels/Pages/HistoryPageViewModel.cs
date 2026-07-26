using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DotX.Desktop.Models;

namespace DotX.Desktop.ViewModels.Pages;

public partial class HistoryPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<PromptHistoryModel> _prompts;

    public HistoryPageViewModel()
    {
        _prompts = new ObservableCollection<PromptHistoryModel>
        {
            new PromptHistoryModel { PromptText = "Create a React component for a responsive navigation bar with a dark mode toggle.", Timestamp = "Today, 10:23 AM", AgentUsed = "Frontend Architect", Status = "Success" },
            new PromptHistoryModel { PromptText = "Write a SQL query to find the top 5 customers who have spent the most in the last quarter.", Timestamp = "Yesterday, 3:45 PM", AgentUsed = "Data Engineer", Status = "Success" },
            new PromptHistoryModel { PromptText = "Debug this Python script that keeps throwing an IndexError in the loop.", Timestamp = "Yesterday, 1:12 PM", AgentUsed = "Code Reviewer", Status = "Failed" },
            new PromptHistoryModel { PromptText = "Generate a boilerplate for an Express.js API with JWT authentication.", Timestamp = "Oct 24, 9:00 AM", AgentUsed = "Backend Architect", Status = "Success" },
            new PromptHistoryModel { PromptText = "Optimize the Dockerfile to reduce image size and improve build caching.", Timestamp = "Oct 23, 4:30 PM", AgentUsed = "DevOps Engineer", Status = "Pending" }
        };
    }
}
