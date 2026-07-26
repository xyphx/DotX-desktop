using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DotX.Desktop.Messages;

public class AppNotificationMessage : ValueChangedMessage<string>
{
    public AppNotificationMessage(string value) : base(value)
    {
    }
}
