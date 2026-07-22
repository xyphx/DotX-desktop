using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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
                FileName = "PRD_SmartTask.pdf", 
                FileSize = "1.2 MB", 
                IsUploaded = true 
            },
            new DocumentModel { 
                Name = "2. Functional Requirement Document", 
                Title = "FRD", 
                Description = "Contains all functional requirements, use cases, user stories, workflows, and rules.", 
                FileName = "FRD_SmartTask.pdf", 
                FileSize = "2.8 MB", 
                IsUploaded = true 
            },
            new DocumentModel { 
                Name = "3. Non-Functional Requirement Document", 
                Title = "NFRD", 
                Description = "Contains UI/UX guidelines, performance, security, scalability, compliance, tech constraints, and other NFRs.", 
                FileName = "NFRD_SmartTask.pdf", 
                FileSize = "1.6 MB", 
                IsUploaded = true 
            }
        };
    }
}
