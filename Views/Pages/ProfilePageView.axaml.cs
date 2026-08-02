using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DotX.Desktop.Views.Pages;

public partial class ProfilePageView : UserControl
{
    public ProfilePageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
