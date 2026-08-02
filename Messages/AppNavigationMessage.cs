using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DotX.Desktop.Messages;

public class AppNavigationMessage : ValueChangedMessage<string>
{
    public AppNavigationMessage(string value) : base(value)
    {
    }
}
