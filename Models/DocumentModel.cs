using CommunityToolkit.Mvvm.ComponentModel;

namespace DotX.Desktop.Models;

public partial class DocumentModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _status = "Required";

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fileSize = string.Empty;

    [ObservableProperty]
    private string _fileId = string.Empty;

    [ObservableProperty]
    private string _fileUrl = string.Empty;

    [ObservableProperty]
    private bool _isUploaded = false;

    [ObservableProperty]
    private bool _isUploading = false;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    partial void OnErrorMessageChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
    }
}
