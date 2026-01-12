using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using HC.Documents;
using HC.DocumentFiles;
using HC.MasterDatas;
using HC.Permissions;
using HC.Shared;
using Volo.Abp.BlobStoring;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp;
using Volo.Abp.Content;
using Microsoft.Extensions.Logging;

namespace HC.Blazor.Pages;

public partial class Documents
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<DocumentWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<DocumentWithNavigationPropertiesDto> DocumentList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateDocument { get; set; }

    private bool CanEditDocument { get; set; }

    private bool CanDeleteDocument { get; set; }

    private DocumentCreateDto NewDocument { get; set; }

    private Validations NewDocumentValidations { get; set; } = new();
    private DocumentUpdateDto EditingDocument { get; set; }

    private Validations EditingDocumentValidations { get; set; } = new();
    private Guid EditingDocumentId { get; set; }

    private Modal CreateDocumentModal { get; set; } = new();
    private Modal EditDocumentModal { get; set; } = new();
    private GetDocumentsInput Filter { get; set; }

    private DataGridEntityActionsColumn<DocumentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "document-create-tab";
    protected string SelectedEditTab = "document-edit-tab";
    private DocumentWithNavigationPropertiesDto? SelectedDocument;

    private IReadOnlyList<LookupDto<Guid>> MasterDatasCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> UnitsCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> WorkflowsCollection { get; set; } = new List<LookupDto<Guid>>();
    
    // MasterData collections for Select2 filters
    private IReadOnlyList<LookupDto<Guid>> TypeMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> UrgencyLevelMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> SecrecyLevelMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> FieldMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> StatusMasterDataCollection { get; set; } = new List<LookupDto<Guid>>();
    
    // Selected values for Select2 filters
    private List<LookupDto<Guid>> SelectedTypeMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedUrgencyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedSecrecyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedFieldMasterData { get; set; } = new();
    private List<LookupDto<Guid>> SelectedStatusMasterData { get; set; } = new();
    
    private List<DocumentWithNavigationPropertiesDto> SelectedDocuments { get; set; } = new();
    private bool AllDocumentsSelected { get; set; }

    // View File Modal
    private Modal? ViewFileModal { get; set; }
    private DocumentWithNavigationPropertiesDto? ViewDocument { get; set; }
    private DocumentDto? ViewDocumentData { get; set; }
    private IReadOnlyList<DocumentFileWithNavigationPropertiesDto> ViewDocumentFilesList { get; set; } = new List<DocumentFileWithNavigationPropertiesDto>();
    private string? ViewPdfFileUrl { get; set; }
    private bool ViewIsPdfFile { get; set; }
    private bool IsLoadingViewDocument { get; set; }
    
    // Selected values for View Modal Select2
    private List<LookupDto<Guid>> ViewSelectedTypeMasterData { get; set; } = new();
    private List<LookupDto<Guid>> ViewSelectedUrgencyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> ViewSelectedSecrecyLevelMasterData { get; set; } = new();
    private List<LookupDto<Guid>> ViewSelectedFieldMasterData { get; set; } = new();
    private List<LookupDto<Guid>> ViewSelectedStatusMasterData { get; set; } = new();
    private List<LookupDto<Guid>> ViewSelectedUnit { get; set; } = new();

    public Documents()
    {
        NewDocument = new DocumentCreateDto();
        EditingDocument = new DocumentUpdateDto();
        Filter = new GetDocumentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        DocumentList = new List<DocumentWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        // Filter by current user's documents only
        if (CurrentUser.Id.HasValue)
        {
            Filter.CreatorId = CurrentUser.Id.Value;
        }
        
        // Load lookup data for Select2 filters
        await LoadLookupDataAsync();
    }
    
    private async Task LoadLookupDataAsync()
    {
        await Task.WhenAll(
            GetTypeMasterDataLookupAsync(TypeMasterDataCollection, "", CancellationToken.None),
            GetUrgencyLevelMasterDataLookupAsync(UrgencyLevelMasterDataCollection, "", CancellationToken.None),
            GetSecrecyLevelMasterDataLookupAsync(SecrecyLevelMasterDataCollection, "", CancellationToken.None),
            GetFieldMasterDataLookupAsync(FieldMasterDataCollection, "", CancellationToken.None),
            GetStatusMasterDataLookupAsync(StatusMasterDataCollection, "", CancellationToken.None),
            GetUnitCollectionLookupAsync(null),
            GetWorkflowCollectionLookupAsync(null)
        );
    }
    
    // MasterData lookup by Type using MasterDatasAppService
    private async Task<List<LookupDto<Guid>>> GetTypeMasterDataLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Type = MasterDataType.DocumentType.GetTypeValue(),
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
            Type = MasterDataType.UrgencyLevel.GetTypeValue(),
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
            Type = MasterDataType.SecrecyLevel.GetTypeValue(),
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
            Type = MasterDataType.Field.GetTypeValue(),
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
            Type = MasterDataType.Status.GetTypeValue(),
            FilterText = filter,
            MaxResultCount = 1000,
            SkipCount = 0
        });
        StatusMasterDataCollection = result.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.Name }).ToList();
        return StatusMasterDataCollection.ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Documents"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewDocument"], async () => {
            NavigationManager.NavigateTo("/document-detail");
        }, IconName.Add, requiredPolicyName: HCPermissions.Documents.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(DocumentWithNavigationPropertiesDto document)
    {
        DataGridRef.ToggleDetailRow(document, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<DocumentWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteDocument;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<DocumentWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.Documents.Create);
        CanEditDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.Documents.Edit);
        CanDeleteDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.Documents.Delete);
    }

    private async Task GetDocumentsAsync()
    {
        // Ensure CreatorId filter is always set for current user
        if (CurrentUser.Id.HasValue && !Filter.CreatorId.HasValue)
        {
            Filter.CreatorId = CurrentUser.Id.Value;
        }
        
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await DocumentsAppService.GetListAsync(Filter);
        DocumentList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetDocumentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await DocumentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/documents/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&No={HttpUtility.UrlEncode(Filter.No)}&Title={HttpUtility.UrlEncode(Filter.Title)}&CurrentStatus={HttpUtility.UrlEncode(Filter.CurrentStatus)}&CompletedTimeMin={Filter.CompletedTimeMin?.ToString("O")}&CompletedTimeMax={Filter.CompletedTimeMax?.ToString("O")}&StorageNumber={HttpUtility.UrlEncode(Filter.StorageNumber)}&FieldId={Filter.FieldId}&UnitId={Filter.UnitId}&WorkflowId={Filter.WorkflowId}&StatusId={Filter.StatusId}&TypeId={Filter.TypeId}&UrgencyLevelId={Filter.UrgencyLevelId}&SecrecyLevelId={Filter.SecrecyLevelId}&CreatorId={Filter.CreatorId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DocumentWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetDocumentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateDocumentModalAsync()
    {
        NewDocument = new DocumentCreateDto
        {
            CompletedTime = DateTime.Now,
        };
        SelectedCreateTab = "document-create-tab";
        await NewDocumentValidations.ClearAll();
        await CreateDocumentModal.Show();
    }

    private async Task CloseCreateDocumentModalAsync()
    {
        NewDocument = new DocumentCreateDto
        {
            CompletedTime = DateTime.Now,
        };
        await CreateDocumentModal.Hide();
    }

    private async Task OpenEditDocumentModalAsync(DocumentWithNavigationPropertiesDto input)
    {
        NavigationManager.NavigateTo($"/document-detail/{input.Document.Id}");
    }

    private DocumentWithNavigationPropertiesDto? DocumentToDelete { get; set; }
    private Modal? DeleteConfirmModal { get; set; }

    private async Task DeleteDocumentAsync(DocumentWithNavigationPropertiesDto input)
    {
        try
        {
            DocumentToDelete = input;
            if (DeleteConfirmModal != null)
            {
                await DeleteConfirmModal.Show();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ConfirmDeleteDocumentAsync()
    {
        try
        {
            if (DeleteConfirmModal != null)
            {
                await DeleteConfirmModal.Hide();
            }

            if (DocumentToDelete != null)
            {
                await DocumentsAppService.DeleteAsync(DocumentToDelete.Document.Id);
                await GetDocumentsAsync();
                DocumentToDelete = null;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CancelDeleteDocumentAsync()
    {
        if (DeleteConfirmModal != null)
        {
            await DeleteConfirmModal.Hide();
        }
        DocumentToDelete = null;
    }

    private async Task SendDocumentAsync(DocumentWithNavigationPropertiesDto input)
    {
        try
        {
            // TODO: Implement send document logic
            await UiMessageService.Info(L["Send"] + ": " + input.Document.No);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task SignDocumentAsync(DocumentWithNavigationPropertiesDto input)
    {
        try
        {
            // TODO: Implement sign document logic
            await UiMessageService.Info(L["Action.SubmitForSigning"] + ": " + input.Document.No);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ViewFileAsync(DocumentWithNavigationPropertiesDto input)
    {
        try
        {
            // Clear previous data
            ViewDocument = null;
            ViewDocumentData = null;
            ViewDocumentFilesList = new List<DocumentFileWithNavigationPropertiesDto>();
            ViewPdfFileUrl = null;
            ViewIsPdfFile = false;
            ViewSelectedTypeMasterData.Clear();
            ViewSelectedUrgencyLevelMasterData.Clear();
            ViewSelectedSecrecyLevelMasterData.Clear();
            ViewSelectedFieldMasterData.Clear();
            ViewSelectedStatusMasterData.Clear();
            ViewSelectedUnit.Clear();
            
            // Set loading state and show modal immediately
            IsLoadingViewDocument = true;
            if (ViewFileModal != null)
            {
                await ViewFileModal.Show();
            }
            
            // Load data after modal is shown
            await LoadDocumentForViewAsync(input.Document.Id);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            // Close modal on error and reset loading state
            IsLoadingViewDocument = false;
            if (ViewFileModal != null)
            {
                await ViewFileModal.Hide();
            }
        }
    }

    private async Task LoadDocumentForViewAsync(Guid documentId)
    {
        // IsLoadingViewDocument is already set to true in ViewFileAsync
        try
        {
            // Load lookup data if not already loaded
            if (!TypeMasterDataCollection.Any() || !FieldMasterDataCollection.Any() || !UnitsCollection.Any())
            {
                await LoadLookupDataAsync();
            }
            
            ViewDocument = await DocumentsAppService.GetWithNavigationPropertiesAsync(documentId);
            if (ViewDocument != null)
            {
                ViewDocumentData = ViewDocument.Document;

                // Load selected values for Select2
                if (ViewDocument.Document.TypeId != default)
                {
                    var typeData = await GetMasterDataByIdAsync(ViewDocument.Document.TypeId, MasterDataType.DocumentType);
                    if (typeData != null)
                        ViewSelectedTypeMasterData = new List<LookupDto<Guid>> { typeData };
                }
                if (ViewDocument.Document.UrgencyLevelId != default)
                {
                    var urgencyData = await GetMasterDataByIdAsync(ViewDocument.Document.UrgencyLevelId, MasterDataType.UrgencyLevel);
                    if (urgencyData != null)
                        ViewSelectedUrgencyLevelMasterData = new List<LookupDto<Guid>> { urgencyData };
                }
                if (ViewDocument.Document.SecrecyLevelId != default)
                {
                    var secrecyData = await GetMasterDataByIdAsync(ViewDocument.Document.SecrecyLevelId, MasterDataType.SecrecyLevel);
                    if (secrecyData != null)
                        ViewSelectedSecrecyLevelMasterData = new List<LookupDto<Guid>> { secrecyData };
                }
                if (ViewDocument.Document.FieldId.HasValue)
                {
                    var fieldData = await GetMasterDataByIdAsync(ViewDocument.Document.FieldId.Value, MasterDataType.Field);
                    if (fieldData != null)
                        ViewSelectedFieldMasterData = new List<LookupDto<Guid>> { fieldData };
                }
                if (ViewDocument.Document.StatusId.HasValue)
                {
                    var statusData = await GetMasterDataByIdAsync(ViewDocument.Document.StatusId.Value, MasterDataType.Status);
                    if (statusData != null)
                        ViewSelectedStatusMasterData = new List<LookupDto<Guid>> { statusData };
                }
                if (ViewDocument.Document.UnitId.HasValue)
                {
                    var unitData = await GetUnitByIdAsync(ViewDocument.Document.UnitId.Value);
                    if (unitData != null)
                        ViewSelectedUnit = new List<LookupDto<Guid>> { unitData };
                }

                // Load document files
                var filesResult = await DocumentFilesAppService.GetListAsync(new GetDocumentFilesInput
                {
                    DocumentId = documentId,
                    MaxResultCount = 1000,
                    SkipCount = 0
                });
                ViewDocumentFilesList = filesResult.Items;

                // Load PDF URL if file exists and is PDF
                await LoadViewPdfUrlAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoadingViewDocument = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task LoadViewPdfUrlAsync()
    {
        ViewPdfFileUrl = null;
        ViewIsPdfFile = false;

        var firstFile = ViewDocumentFilesList.FirstOrDefault();
        if (firstFile == null || string.IsNullOrEmpty(firstFile.DocumentFile.Path))
        {
            return;
        }

        if (!IsPdfFileExtension(firstFile.DocumentFile.Name))
        {
            return;
        }

        try
        {
            ViewIsPdfFile = true;
            var fileBytes = await BlobContainer.GetAllBytesAsync(firstFile.DocumentFile.Path);
            var base64 = Convert.ToBase64String(fileBytes);
            ViewPdfFileUrl = $"data:application/pdf;base64,{base64}";
        }
        catch
        {
            // Silently fail - PDF viewer will just not show
            ViewIsPdfFile = false;
            ViewPdfFileUrl = null;
        }
    }

    private async Task<LookupDto<Guid>?> GetMasterDataByIdAsync(Guid id, MasterDataType type)
    {
        var result = await MasterDatasAppService.GetListAsync(new GetMasterDatasInput
        {
            Type = type.GetTypeValue(),
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

    private async Task CloseViewFileModalAsync()
    {
        if (ViewFileModal != null)
        {
            await ViewFileModal.Hide();
        }
        // Clear data
        ViewDocument = null;
        ViewDocumentData = null;
        ViewDocumentFilesList = new List<DocumentFileWithNavigationPropertiesDto>();
        ViewPdfFileUrl = null;
        ViewIsPdfFile = false;
        ViewSelectedTypeMasterData.Clear();
        ViewSelectedUrgencyLevelMasterData.Clear();
        ViewSelectedSecrecyLevelMasterData.Clear();
        ViewSelectedFieldMasterData.Clear();
        ViewSelectedStatusMasterData.Clear();
        ViewSelectedUnit.Clear();
    }

    private bool HasPdfFile(DocumentWithNavigationPropertiesDto document)
    {
        // For now, return true to show the button
        // In a real implementation, you might want to:
        // 1. Load document files and check if any file has .pdf extension
        // 2. Or add a property to DocumentDto that indicates if it has PDF files
        // 3. Or cache this information when loading documents
        return true; // TODO: Implement proper check by loading document files if needed
    }

    private bool IsPdfFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;
        
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".pdf";
    }

    private async Task CreateDocumentAsync()
    {
        try
        {
            if (await NewDocumentValidations.ValidateAll() == false)
            {
                return;
            }

            await DocumentsAppService.CreateAsync(NewDocument);
            await GetDocumentsAsync();
            await CloseCreateDocumentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditDocumentModalAsync()
    {
        await EditDocumentModal.Hide();
    }

    private async Task UpdateDocumentAsync()
    {
        try
        {
            if (await EditingDocumentValidations.ValidateAll() == false)
            {
                return;
            }

            await DocumentsAppService.UpdateAsync(EditingDocumentId, EditingDocument);
            await GetDocumentsAsync();
            await EditDocumentModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        SelectedCreateTab = name;
    }

    private void OnSelectedEditTabChanged(string name)
    {
        SelectedEditTab = name;
    }

    protected virtual async Task OnNoChangedAsync(string? no)
    {
        Filter.No = no;
        await SearchAsync();
    }

    protected virtual async Task OnTitleChangedAsync(string? title)
    {
        Filter.Title = title;
        await SearchAsync();
    }

    protected virtual async Task OnCurrentStatusChangedAsync(string? currentStatus)
    {
        Filter.CurrentStatus = currentStatus;
        await SearchAsync();
    }

    protected virtual async Task OnCompletedTimeMinChangedAsync(DateTime? completedTimeMin)
    {
        Filter.CompletedTimeMin = completedTimeMin.HasValue ? completedTimeMin.Value.Date : completedTimeMin;
        await SearchAsync();
    }

    protected virtual async Task OnCompletedTimeMaxChangedAsync(DateTime? completedTimeMax)
    {
        Filter.CompletedTimeMax = completedTimeMax.HasValue ? completedTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : completedTimeMax;
        await SearchAsync();
    }

    protected virtual async Task OnStorageNumberChangedAsync(string? storageNumber)
    {
        Filter.StorageNumber = storageNumber;
        await SearchAsync();
    }

    // Select2 change handlers for filters
    private void OnTypeIdChanged()
    {
        if (SelectedTypeMasterData?.Any() == true)
        {
            Filter.TypeId = SelectedTypeMasterData[0].Id;
        }
        else
        {
            Filter.TypeId = null;
        }
        InvokeAsync(async () => await SearchAsync());
    }

    private void OnUrgencyLevelIdChanged()
    {
        if (SelectedUrgencyLevelMasterData?.Any() == true)
        {
            Filter.UrgencyLevelId = SelectedUrgencyLevelMasterData[0].Id;
        }
        else
        {
            Filter.UrgencyLevelId = null;
        }
        InvokeAsync(async () => await SearchAsync());
    }

    private void OnSecrecyLevelIdChanged()
    {
        if (SelectedSecrecyLevelMasterData?.Any() == true)
        {
            Filter.SecrecyLevelId = SelectedSecrecyLevelMasterData[0].Id;
        }
        else
        {
            Filter.SecrecyLevelId = null;
        }
        InvokeAsync(async () => await SearchAsync());
    }

    private void OnFieldIdChanged()
    {
        if (SelectedFieldMasterData?.Any() == true)
        {
            Filter.FieldId = SelectedFieldMasterData[0].Id;
        }
        else
        {
            Filter.FieldId = null;
        }
        InvokeAsync(async () => await SearchAsync());
    }

    private void OnStatusIdChanged()
    {
        if (SelectedStatusMasterData?.Any() == true)
        {
            Filter.StatusId = SelectedStatusMasterData[0].Id;
        }
        else
        {
            Filter.StatusId = null;
        }
        InvokeAsync(async () => await SearchAsync());
    }

    protected virtual async Task OnTypeIdChangedAsync(Guid? typeId)
    {
        Filter.TypeId = typeId;
        await SearchAsync();
    }

    protected virtual async Task OnUrgencyLevelIdChangedAsync(Guid? urgencyLevelId)
    {
        Filter.UrgencyLevelId = urgencyLevelId;
        await SearchAsync();
    }

    protected virtual async Task OnSecrecyLevelIdChangedAsync(Guid? secrecyLevelId)
    {
        Filter.SecrecyLevelId = secrecyLevelId;
        await SearchAsync();
    }

    protected virtual async Task OnFieldIdChangedAsync(Guid? fieldId)
    {
        Filter.FieldId = fieldId;
        await SearchAsync();
    }

    protected virtual async Task OnStatusIdChangedAsync(Guid? statusId)
    {
        Filter.StatusId = statusId;
        await SearchAsync();
    }

    protected virtual async Task OnUnitIdChangedAsync(Guid? unitId)
    {
        Filter.UnitId = unitId;
        await SearchAsync();
    }

    protected virtual async Task OnWorkflowIdChangedAsync(Guid? workflowId)
    {
        Filter.WorkflowId = workflowId;
        await SearchAsync();
    }

    private async Task GetMasterDataCollectionLookupAsync(string? newValue = null)
    {
        MasterDatasCollection = (await DocumentsAppService.GetMasterDataLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetUnitCollectionLookupAsync(string? newValue = null)
    {
        UnitsCollection = (await DocumentsAppService.GetUnitLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetWorkflowCollectionLookupAsync(string? newValue = null)
    {
        WorkflowsCollection = (await DocumentsAppService.GetWorkflowLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllDocumentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllDocumentsSelected = false;
        SelectedDocuments.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedDocumentRowsChanged()
    {
        if (SelectedDocuments.Count != PageSize)
        {
            AllDocumentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedDocumentsAsync()
    {
        var message = AllDocumentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDocuments.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllDocumentsSelected)
        {
            await DocumentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await DocumentsAppService.DeleteByIdsAsync(SelectedDocuments.Select(x => x.Document.Id).ToList());
        }

        SelectedDocuments.Clear();
        AllDocumentsSelected = false;
        await GetDocumentsAsync();
    }
}