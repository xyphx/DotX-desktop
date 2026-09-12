using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotX.Desktop.Messages;
using DotX.Desktop.Models;
using DotX.Desktop.Services;

namespace DotX.Desktop.ViewModels.Components;

public partial class DocumentUploadViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<DocumentModel> _documents;

    public DocumentUploadViewModel()
    {
        _documents = new ObservableCollection<DocumentModel>
        {
            new DocumentModel { 
                Name = "1. Product Requirement Document", 
                Title = "PRD", 
                Description = "Contains product overview, goals, features summary, target audience, and vision.", 
                FileName = "", 
                FileSize = "", 
                IsUploaded = false 
            },
            new DocumentModel { 
                Name = "2. Functional Requirement Document", 
                Title = "FRD", 
                Description = "Contains all functional requirements, use cases, user stories, workflows, and rules.", 
                FileName = "", 
                FileSize = "", 
                IsUploaded = false 
            },
            new DocumentModel { 
                Name = "3. Non-Functional Requirement Document", 
                Title = "NFRD", 
                Description = "Contains UI/UX guidelines, performance, security, scalability, compliance, tech constraints, and other NFRs.", 
                FileName = "", 
                FileSize = "", 
                IsUploaded = false 
            }
        };
    }

    [RelayCommand]
    public async Task SelectFileAsync(DocumentModel document)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = $"Select {document.Name}",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Supported Documents (*.pdf, *.docx, *.txt)")
                        {
                            Patterns = new[] { "*.pdf", "*.docx", "*.doc", "*.txt" }
                        },
                        FilePickerFileTypes.All
                    }
                });

                if (files.Count >= 1)
                {
                    var file = files[0];
                    document.FileName = file.Name;
                    document.ErrorMessage = string.Empty;
                    document.IsUploading = true;

                    try
                    {
                        var props = await file.GetBasicPropertiesAsync();
                        if (props.Size.HasValue)
                        {
                            document.FileSize = $"{(props.Size.Value / 1024.0 / 1024.0):F1} MB";
                        }
                        else
                        {
                            document.FileSize = "Unknown Size";
                        }

                        await using var stream = await file.OpenReadAsync();
                        var response = await ApiGatewayClient.Instance.UploadFileAsync("/api/storage/upload", stream, file.Name);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            using var doc = JsonDocument.Parse(responseBody);
                            var root = doc.RootElement;
                            if (root.TryGetProperty("fileId", out var fidProp))
                            {
                                document.FileId = fidProp.GetString() ?? string.Empty;
                            }
                            if (root.TryGetProperty("url", out var urlProp))
                            {
                                document.FileUrl = urlProp.GetString() ?? string.Empty;
                            }

                            document.IsUploaded = true;
                            document.Status = "Uploaded";
                            WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"Successfully uploaded {document.Title}!"));
                        }
                        else
                        {
                            document.IsUploaded = false;
                            document.ErrorMessage = ErrorFormatter.Format(responseBody, response.StatusCode);
                            WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"Upload failed: {document.ErrorMessage}"));
                        }
                    }
                    catch (Exception ex)
                    {
                        document.IsUploaded = false;
                        document.ErrorMessage = ErrorFormatter.Format(ex.Message);
                        WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"Error uploading: {document.ErrorMessage}"));
                    }
                    finally
                    {
                        document.IsUploading = false;
                    }
                }
            }
        }
    }

    [RelayCommand]
    public void Validate()
    {
        var uploadedCount = Documents.Count(d => d.IsUploaded);
        if (uploadedCount == Documents.Count)
        {
            WeakReferenceMessenger.Default.Send(new AppNotificationMessage("All 3 requirement documents are uploaded and verified. Ready to proceed!"));
        }
        else
        {
            var missing = Documents.Where(d => !d.IsUploaded).Select(d => d.Title);
            WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"Missing required documents: {string.Join(", ", missing)}. Please upload them to continue."));
        }
    }

    [RelayCommand]
    public void Proceed()
    {
        var uploadedCount = Documents.Count(d => d.IsUploaded);
        if (uploadedCount == Documents.Count)
        {
            WeakReferenceMessenger.Default.Send(new AppNavigationMessage("AI Agents"));
            WeakReferenceMessenger.Default.Send(new AppNotificationMessage("Requirements confirmed! Advancing to AI Agents."));
        }
        else
        {
            var missing = Documents.Where(d => !d.IsUploaded).Select(d => d.Title);
            WeakReferenceMessenger.Default.Send(new AppNotificationMessage($"Please upload all required documents ({string.Join(", ", missing)}) before proceeding."));
        }
    }
}
