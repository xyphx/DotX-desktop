namespace DotX.Desktop.Models;

public class DocumentModel
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Required";
    public string FileName { get; set; } = string.Empty;
    public string FileSize { get; set; } = string.Empty;
    public bool IsUploaded { get; set; } = false;
}
