using CommunityToolkit.Mvvm.ComponentModel;

namespace DotX.Desktop.Models;

public partial class NavItemModel : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    
    [ObservableProperty]
    private bool _isActive = false;
}
