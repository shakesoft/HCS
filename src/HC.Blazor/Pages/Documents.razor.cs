using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

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
    private List<DocumentWithNavigationPropertiesDto> SelectedDocuments { get; set; } = new();
    private bool AllDocumentsSelected { get; set; }

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

    private async Task DeleteDocumentAsync(DocumentWithNavigationPropertiesDto input)
    {
        try
        {
            await DocumentsAppService.DeleteAsync(input.Document.Id);
            await GetDocumentsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
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

    protected virtual async Task OnFieldIdChangedAsync(Guid? fieldId)
    {
        Filter.FieldId = fieldId;
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

    protected virtual async Task OnStatusIdChangedAsync(Guid? statusId)
    {
        Filter.StatusId = statusId;
        await SearchAsync();
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