using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotX.Desktop.Models;

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
                    Title = "Select Document",
                    AllowMultiple = false
                });

                if (files.Count >= 1)
                {
                    var file = files[0];
                    document.FileName = file.Name;
                    
                    var props = await file.GetBasicPropertiesAsync();
                    if (props.Size.HasValue)
                    {
                        document.FileSize = $"{(props.Size.Value / 1024.0 / 1024.0):F1} MB";
                    }
                    else
                    {
                        document.FileSize = "Unknown Size";
                    }
                    
                    document.IsUploaded = true;
                }
            }
        }
    }

    [RelayCommand]
    public void Validate()
    {
        // Simple mock behavior
        Console.WriteLine("Validating documents...");
    }

    [RelayCommand]
    public void Proceed()
    {
        // Simple mock behavior
        Console.WriteLine("Proceeding to Agents...");
    }
}
