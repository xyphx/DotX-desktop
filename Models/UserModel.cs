using System;

namespace DotX.Desktop.Models;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public string AuthProvider { get; set; } = string.Empty;

    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name)) return "U";
            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0][..1].ToUpper();
            return (parts[0][..1] + parts[^1][..1]).ToUpper();
        }
    }
}
