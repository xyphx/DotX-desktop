using CommunityToolkit.Mvvm.ComponentModel;

namespace DotX.Desktop.Models;

public partial class ProjectModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _lastModified = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    [ObservableProperty]
    private string _icon = string.Empty;
}
