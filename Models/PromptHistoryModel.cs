using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DotX.Desktop.Models;

public partial class PromptHistoryModel : ObservableObject
{
    [ObservableProperty]
    private string _promptText = string.Empty;

    [ObservableProperty]
    private string _timestamp = string.Empty;

    [ObservableProperty]
    private string _agentUsed = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;
}
