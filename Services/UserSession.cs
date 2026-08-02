using DotX.Desktop.Models;

namespace DotX.Desktop.Services;

public class UserSession
{
    public static UserSession Instance { get; } = new();

    public UserModel? CurrentUser { get; set; }

    public bool IsLoggedIn => CurrentUser != null;
}
