using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Blazorise;
using HC.Documents;
using HC.DocumentFiles;
using HC.MasterDatas;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Volo.Abp.BlobStoring;
using Volo.Abp.Application.Dtos;

namespace HC.Blazor.Pages;

public partial class DocumentDetail : HCComponentBase
{
    [Parameter] public Guid DocumentId { get; set; }

    [SupplyParameterFromQuery(Name = "id")]
    public Guid? DocumentIdQuery { get; set; }

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems { get; } = new();

    protected string PageTitle => IsEditMode
        ? (CurrentDocument?.Document is null ? L["Documents"] : L["EditDocument"])
        : L["NewDocument"];

    protected bool IsLoading { get; set; }
    protected bool IsEditMode => DocumentId != Guid.Empty;
    protected bool IsViewMode { get; set; } = false;
    protected DocumentWithNavigationPropertiesDto? CurrentDocument { get; set; }

    private bool CanEditDocument { get; set; }
    private bool CanCreateDocument { get; set; }
    private bool CanDeleteDocumentFile { get; set; }

    // Document data
    private DocumentCreateDto? DocumentCreateData { get; set; }
    private DocumentUpdateDto? DocumentUpdateData { get; set; }

    private Validations DocumentValidations { get; set; } = new();

    // MasterData collections
    private IReadOnlyList<LookupDto<Guid>> TypeMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> UrgencyLevelMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> SecrecyLevelMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> FieldMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> StatusMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> UnitsCollection { get; set; } = new List<LookupDto<Guid>>();

    // Selected values for Select2
    private List<LookupDto<Guid>> SelectedTypeMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedUrgencyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedSecrecyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedFieldMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedStatusMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedUnit { get; set; } = new();

    // File upload
    private IFileEntry? SelectedFile { get; set; }
    private string UploadedFilePath { get; set; } = string.Empty;
    private string UploadedFileHash { get; set; } = string.Empty;
    private bool IsUploading { get; set; }
    private int FilePickerProgress { get; set; }

    // Document files list
    private IReadOnlyList<DocumentFileWithNavigationPropertiesDto> DocumentFilesList { get; set; } = new List<DocumentFileWithNavigationPropertiesDto>();

    private Guid _loadedDocumentId;

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        Logger.LogInformation($"OnParametersSetAsync called. DocumentId: {DocumentId}, DocumentIdQuery: {DocumentIdQuery}, _loadedDocumentId: {_loadedDocumentId}");
        
        if (DocumentId == Guid.Empty && DocumentIdQuery.HasValue)
        {
            DocumentId = DocumentIdQuery.Value;
            Logger.LogInformation($"DocumentId set from query: {DocumentId}");
        }

        if (DocumentId == Guid.Empty)
        {
            // Create mode
            Logger.LogInformation("OnParametersSetAsync: Create mode");
            InitializeCreateMode();
        }
        else
        {
            if (_loadedDocumentId == DocumentId)
            {
                Logger.LogInformation($"OnParametersSetAsync: Document already loaded, skipping. DocumentId: {DocumentId}");
                BreadcrumbItems.Clear();
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Documents"], "/documents"));
                BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(IsEditMode ? L["Details"] : L["NewDocument"]));
                await LoadLookupDataAsync();
                await InvokeAsync(StateHasChanged);
                return;
            }

            Logger.LogInformation($"OnParametersSetAsync: Loading document. DocumentId: {DocumentId}");
            _loadedDocumentId = DocumentId;
            await LoadDocumentAsync();
        }

        BreadcrumbItems.Clear();
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Documents"], "/documents"));
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(IsEditMode ? L["Details"] : L["NewDocument"]));

        await LoadLookupDataAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.Documents.Create);
        CanEditDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.Documents.Edit);
        CanDeleteDocumentFile = await AuthorizationService.IsGrantedAsync(HCPermissions.DocumentFiles.Delete);
    }

    private void InitializeCreateMode()
    {
        DocumentCreateData = new DocumentCreateDto
        {
            CompletedTime = DateTime.Now
        };
        DocumentUpdateData = null;
    }

    private async Task LoadDocumentAsync()
    {
        IsLoading = true;
        try
        {
            Logger.LogInformation($"LoadDocumentAsync called. DocumentId: {DocumentId}");
            CurrentDocument = await DocumentsAppService.GetWithNavigationPropertiesAsync(DocumentId);
            if (CurrentDocument != null)
            {
                Logger.LogInformation($"LoadDocumentAsync: Document loaded successfully");
                DocumentUpdateData = ObjectMapper.Map<DocumentDto, DocumentUpdateDto>(CurrentDocument.Document);
                DocumentCreateData = null;

                // Load selected values for Select2
                if (CurrentDocument.Document.TypeId != default)
                {
                    var typeData = await GetMasterDataByIdAsync(CurrentDocument.Document.TypeId, "LOAI_VB");
                    if (typeData != null)
                        SelectedTypeMasterData = new List<LookupDto<Guid>> { typeData };
                }
                if (CurrentDocument.Document.UrgencyLevelId != default)
                {
                    var urgencyData = await GetMasterDataByIdAsync(CurrentDocument.Document.UrgencyLevelId, "MUC_DO_KHAN");
                    if (urgencyData != null)
                        SelectedUrgencyLevelMasterData = new List<LookupDto<Guid>> { urgencyData };
                }
                if (CurrentDocument.Document.SecrecyLevelId != default)
                {
                    var secrecyData = await GetMasterDataByIdAsync(CurrentDocument.Document.SecrecyLevelId, "MUC_DO_MAT");
                    if (secrecyData != null)
                        SelectedSecrecyLevelMasterData = new List<LookupDto<Guid>> { secrecyData };
                }
                if (CurrentDocument.Document.FieldId.HasValue)
                {
                    var fieldData = await GetMasterDataByIdAsync(CurrentDocument.Document.FieldId.Value, "LINH_VUC_VB");
                    if (fieldData != null)
                        SelectedFieldMasterData = new List<LookupDto<Guid>> { fieldData };
                }
                if (CurrentDocument.Document.StatusId.HasValue)
                {
                    var statusData = await GetMasterDataByIdAsync(CurrentDocument.Document.StatusId.Value, "TRANG_THAI_VB");
                    if (statusData != null)
                        SelectedStatusMasterData = new List<LookupDto<Guid>> { statusData };
                }
                if (CurrentDocument.Document.UnitId.HasValue)
                {
                    var unitData = await GetUnitByIdAsync(CurrentDocument.Document.UnitId.Value);
                    if (unitData != null)
                        SelectedUnit = new List<LookupDto<Guid>> { unitData };
                }

                // Load document files
                Logger.LogInformation($"LoadDocumentAsync: Calling LoadDocumentFilesAsync");
                await LoadDocumentFilesAsync();
            }
            else
            {
                Logger.LogWarning($"LoadDocumentAsync: Document not found. DocumentId: {DocumentId}");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error loading document. DocumentId: {DocumentId}");
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadDocumentFilesAsync()
    {
        try
        {
            Logger.LogInformation($"LoadDocumentFilesAsync called. DocumentId: {DocumentId}");
            
            if (DocumentId == Guid.Empty)
            {
                Logger.LogWarning("LoadDocumentFilesAsync: DocumentId is Empty");
                return;
            }

            var result = await DocumentFilesAppService.GetListAsync(new GetDocumentFilesInput
            {
                DocumentId = DocumentId,
                MaxResultCount = 1000,
                SkipCount = 0
            });
            
            Logger.LogInformation($"DocumentFilesList loaded: {result.Items.Count} items");
            Console.WriteLine($"DocumentFilesList: {result.Items.Count}");
            DocumentFilesList = result.Items;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error loading document files for DocumentId: {DocumentId}");
            throw;
        }
    }

    private async Task LoadLookupDataAsync()
    {
        await Task.WhenAll(
            GetTypeMasterDataLookupAsync(TypeMasterDataCollection, "", CancellationToken.None),
            GetUrgencyLevelMasterDataLookupAsync(UrgencyLevelMasterDataCollection, "", CancellationToken.None),
            GetSecrecyLevelMasterDataLookupAsync(SecrecyLevelMasterDataCollection, "", CancellationToken.None),
            GetFieldMasterDataLookupAsync(FieldMasterDataCollection, "", CancellationToken.None),
            GetStatusMasterDataLookupAsync(StatusMasterDataCollection, "", CancellationToken.None),
            GetUnitLookupAsync(UnitsCollection, "", CancellationToken.None)
        );
    }

    // MasterData lookup by Code using MasterDatasAppService
    private async Task<List<LookupDto<Guid>>> GetTypeMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = "LOAI_VB",
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        TypeMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return TypeMasterDataCollection.ToList();
    }

    private async Task<List<LookupDto<Guid>>> GetUrgencyLevelMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = "MUC_DO_KHAN",
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        UrgencyLevelMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return UrgencyLevelMasterDataCollection.ToList();
    }

    private async Task<List<LookupDto<Guid>>> GetSecrecyLevelMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = "MUC_DO_MAT",
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        SecrecyLevelMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return SecrecyLevelMasterDataCollection.ToList();
    }

    private async Task<List<LookupDto<Guid>>> GetFieldMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = "LINH_VUC_VB",
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        FieldMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return FieldMasterDataCollection.ToList();
    }

    private async Task<List<LookupDto<Guid>>> GetStatusMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = "TRANG_THAI_VB",
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        StatusMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return StatusMasterDataCollection.ToList();
    }

    private async Task<List<LookupDto<Guid>>> GetUnitLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await DocumentsAppService.GetUnitLookupAsync(new LookupRequestDto { Filter = filter });
        UnitsCollection = result.Items;
        return UnitsCollection.ToList();
    }

    private async Task<LookupDto<Guid>?> GetMasterDataByIdAsync(Guid id, string code)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Code = code,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        var masterData = result.Items.FirstOrDefault(x => x.Id == id);
        if (masterData != null)
        {
            return new LookupDto<Guid> { Id = masterData.Id, DisplayName = masterData.Name };
        }
        return null;
    }

    private async Task<LookupDto<Guid>?> GetUnitByIdAsync(Guid id)
    {
        var result = await DocumentsAppService.GetUnitLookupAsync(new LookupRequestDto { Filter = "" });
        return result.Items.FirstOrDefault(x => x.Id == id);
    }

    // Select2 change handlers
    private void OnTypeIdChanged()
    {
        if (SelectedTypeMasterData?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.TypeId = SelectedTypeMasterData[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.TypeId = SelectedTypeMasterData[0].Id;
        }
        InvokeAsync(StateHasChanged);
    }

    private void OnUrgencyLevelIdChanged()
    {
        if (SelectedUrgencyLevelMasterData?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.UrgencyLevelId = SelectedUrgencyLevelMasterData[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.UrgencyLevelId = SelectedUrgencyLevelMasterData[0].Id;
        }
        InvokeAsync(StateHasChanged);
    }

    private void OnSecrecyLevelIdChanged()
    {
        if (SelectedSecrecyLevelMasterData?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.SecrecyLevelId = SelectedSecrecyLevelMasterData[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.SecrecyLevelId = SelectedSecrecyLevelMasterData[0].Id;
        }
        InvokeAsync(StateHasChanged);
    }

    private void OnFieldIdChanged()
    {
        if (SelectedFieldMasterData?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.FieldId = SelectedFieldMasterData[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.FieldId = SelectedFieldMasterData[0].Id;
        }
        else
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.FieldId = null;
            else if (DocumentCreateData != null)
                DocumentCreateData.FieldId = null;
        }
        InvokeAsync(StateHasChanged);
    }

    private void OnStatusIdChanged()
    {
        if (SelectedStatusMasterData?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.StatusId = SelectedStatusMasterData[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.StatusId = SelectedStatusMasterData[0].Id;
        }
        else
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.StatusId = null;
            else if (DocumentCreateData != null)
                DocumentCreateData.StatusId = null;
        }
        InvokeAsync(StateHasChanged);
    }

    private void OnUnitIdChanged()
    {
        if (SelectedUnit?.Any() == true)
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.UnitId = SelectedUnit[0].Id;
            else if (DocumentCreateData != null)
                DocumentCreateData.UnitId = SelectedUnit[0].Id;
        }
        else
        {
            if (IsEditMode && DocumentUpdateData != null)
                DocumentUpdateData.UnitId = null;
            else if (DocumentCreateData != null)
                DocumentCreateData.UnitId = null;
        }
        InvokeAsync(StateHasChanged);
    }

    // File upload handler
    private async Task OnFileUpload(FileUploadEventArgs e)
    {
        try
        {
            IsUploading = true;
            SelectedFile = e.File;
            FilePickerProgress = 0;

            using var memoryStream = new MemoryStream();
            await e.File.OpenReadStream(long.MaxValue).CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileBytes = memoryStream.ToArray();

            // Calculate file hash
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(fileBytes);
            UploadedFileHash = Convert.ToHexString(hashBytes).ToLowerInvariant();

            // Generate unique file name
            var fileName = $"{Guid.NewGuid()}_{e.File.Name}";
            var filePath = $"documents/{fileName}";

            // Upload to MinIO
            await BlobContainer.SaveAsync(filePath, fileBytes);

            UploadedFilePath = filePath;
            FilePickerProgress = 100;

            await UiMessageService.Success(L["FileUploadedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            UploadedFilePath = string.Empty;
            FilePickerProgress = 0;
        }
        finally
        {
            IsUploading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    // Save handler
    private async Task OnSave()
    {
        try
        {
            if (await DocumentValidations.ValidateAll() == false)
            {
                return;
            }

            // Validate required file is uploaded for create mode
            if (!IsEditMode && string.IsNullOrEmpty(UploadedFilePath))
            {
                await UiMessageService.Warn(L["PleaseUploadFileFirst"]);
                return;
            }

            DocumentDto savedDocument;

            if (IsEditMode && DocumentUpdateData != null)
            {
                savedDocument = await DocumentsAppService.UpdateAsync(DocumentId, DocumentUpdateData);
            }
            else if (DocumentCreateData != null)
            {
                savedDocument = await DocumentsAppService.CreateAsync(DocumentCreateData);

                // Save file to DocumentFiles table
                if (!string.IsNullOrEmpty(UploadedFilePath) && SelectedFile != null)
                {
                    await DocumentFilesAppService.CreateAsync(new DocumentFileCreateDto
                    {
                        DocumentId = savedDocument.Id,
                        Name = SelectedFile.Name,
                        Path = UploadedFilePath,
                        Hash = UploadedFileHash,
                        IsSigned = false,
                        UploadedAt = DateTime.Now
                    });
                }
            }
            else
            {
                return;
            }

            await UiMessageService.Success(L["SuccessfullySaved"]);
            NavigationManager.NavigateTo($"/document-detail/{savedDocument.Id}");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnCancel()
    {
        NavigationManager.NavigateTo("/documents");
    }

    private void OnBack()
    {
        NavigationManager.NavigateTo("/documents");
    }

    private void OnEdit()
    {
        IsViewMode = false;
        InvokeAsync(StateHasChanged);
    }

    private async Task DownloadFileAsync(string? filePath, string fileName)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            var fileBytes = await BlobContainer.GetAllBytesAsync(filePath);
            
            // Create blob URL and download using JavaScript
            var base64 = Convert.ToBase64String(fileBytes);
            var contentType = "application/octet-stream";
            var jsCode = $@"
                (function() {{
                    const blob = new Blob([Uint8Array.from(atob('{base64}'), c => c.charCodeAt(0))], {{ type: '{contentType}' }});
                    const url = window.URL.createObjectURL(blob);
                    const link = document.createElement('a');
                    link.href = url;
                    link.download = '{fileName}';
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                    window.URL.revokeObjectURL(url);
                }})();
            ";
            
            await JSRuntime.InvokeVoidAsync("eval", jsCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error downloading file. FilePath: {filePath}, FileName: {fileName}");
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteFileAsync(DocumentFileWithNavigationPropertiesDto file)
    {
        try
        {
            if (!await UiMessageService.Confirm(L["DeleteConfirmationMessage"]))
            {
                return;
            }

            // Delete file from MinIO if path exists
            if (!string.IsNullOrEmpty(file.DocumentFile.Path))
            {
                try
                {
                    await BlobContainer.DeleteAsync(file.DocumentFile.Path);
                    Logger.LogInformation($"File deleted from MinIO: {file.DocumentFile.Path}");
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"Failed to delete file from MinIO: {file.DocumentFile.Path}");
                    // Continue to delete from database even if MinIO deletion fails
                }
            }

            // Delete from DocumentFiles table
            await DocumentFilesAppService.DeleteAsync(file.DocumentFile.Id);

            // Reload document files list
            await LoadDocumentFilesAsync();

            await UiMessageService.Success(L["SuccessfullyDeleted"]);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error deleting file. FileId: {file.DocumentFile.Id}");
            await HandleErrorAsync(ex);
        }
    }
}
