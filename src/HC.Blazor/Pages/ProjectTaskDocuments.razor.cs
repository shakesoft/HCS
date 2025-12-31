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
using HC.ProjectTaskDocuments;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class ProjectTaskDocuments
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<ProjectTaskDocumentWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<ProjectTaskDocumentWithNavigationPropertiesDto> ProjectTaskDocumentList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateProjectTaskDocument { get; set; }

    private bool CanEditProjectTaskDocument { get; set; }

    private bool CanDeleteProjectTaskDocument { get; set; }

    private ProjectTaskDocumentCreateDto NewProjectTaskDocument { get; set; }

    private Validations NewProjectTaskDocumentValidations { get; set; } = new();
    private ProjectTaskDocumentUpdateDto EditingProjectTaskDocument { get; set; }

    private Validations EditingProjectTaskDocumentValidations { get; set; } = new();
    private Guid EditingProjectTaskDocumentId { get; set; }

    private Modal CreateProjectTaskDocumentModal { get; set; } = new();
    private Modal EditProjectTaskDocumentModal { get; set; } = new();
    private GetProjectTaskDocumentsInput Filter { get; set; }

    private DataGridEntityActionsColumn<ProjectTaskDocumentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "projectTaskDocument-create-tab";
    protected string SelectedEditTab = "projectTaskDocument-edit-tab";
    private ProjectTaskDocumentWithNavigationPropertiesDto? SelectedProjectTaskDocument;

    private IReadOnlyList<LookupDto<Guid>> ProjectTasksCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> DocumentsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<ProjectTaskDocumentWithNavigationPropertiesDto> SelectedProjectTaskDocuments { get; set; } = new();
    private bool AllProjectTaskDocumentsSelected { get; set; }

    public ProjectTaskDocuments()
    {
        NewProjectTaskDocument = new ProjectTaskDocumentCreateDto();
        EditingProjectTaskDocument = new ProjectTaskDocumentUpdateDto();
        Filter = new GetProjectTaskDocumentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProjectTaskDocumentList = new List<ProjectTaskDocumentWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ProjectTaskDocuments"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewProjectTaskDocument"], async () => {
            await OpenCreateProjectTaskDocumentModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.ProjectTaskDocuments.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(ProjectTaskDocumentWithNavigationPropertiesDto projectTaskDocument)
    {
        DataGridRef.ToggleDetailRow(projectTaskDocument, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<ProjectTaskDocumentWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteProjectTaskDocument;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<ProjectTaskDocumentWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProjectTaskDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskDocuments.Create);
        CanEditProjectTaskDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskDocuments.Edit);
        CanDeleteProjectTaskDocument = await AuthorizationService.IsGrantedAsync(HCPermissions.ProjectTaskDocuments.Delete);
    }

    private async Task GetProjectTaskDocumentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await ProjectTaskDocumentsAppService.GetListAsync(Filter);
        ProjectTaskDocumentList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProjectTaskDocumentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProjectTaskDocumentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/project-task-documents/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&DocumentPurpose={HttpUtility.UrlEncode(Filter.DocumentPurpose)}&ProjectTaskId={Filter.ProjectTaskId}&DocumentId={Filter.DocumentId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProjectTaskDocumentWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetProjectTaskDocumentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProjectTaskDocumentModalAsync()
    {
        NewProjectTaskDocument = new ProjectTaskDocumentCreateDto
        {
        };
        SelectedCreateTab = "projectTaskDocument-create-tab";
        await NewProjectTaskDocumentValidations.ClearAll();
        await CreateProjectTaskDocumentModal.Show();
    }

    private async Task CloseCreateProjectTaskDocumentModalAsync()
    {
        NewProjectTaskDocument = new ProjectTaskDocumentCreateDto
        {
        };
        await CreateProjectTaskDocumentModal.Hide();
    }

    private async Task OpenEditProjectTaskDocumentModalAsync(ProjectTaskDocumentWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "projectTaskDocument-edit-tab";
        var projectTaskDocument = await ProjectTaskDocumentsAppService.GetWithNavigationPropertiesAsync(input.ProjectTaskDocument.Id);
        EditingProjectTaskDocumentId = projectTaskDocument.ProjectTaskDocument.Id;
        EditingProjectTaskDocument = ObjectMapper.Map<ProjectTaskDocumentDto, ProjectTaskDocumentUpdateDto>(projectTaskDocument.ProjectTaskDocument);
        await EditingProjectTaskDocumentValidations.ClearAll();
        await EditProjectTaskDocumentModal.Show();
    }

    private async Task DeleteProjectTaskDocumentAsync(ProjectTaskDocumentWithNavigationPropertiesDto input)
    {
        try
        {
            await ProjectTaskDocumentsAppService.DeleteAsync(input.ProjectTaskDocument.Id);
            await GetProjectTaskDocumentsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateProjectTaskDocumentAsync()
    {
        try
        {
            if (await NewProjectTaskDocumentValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTaskDocumentsAppService.CreateAsync(NewProjectTaskDocument);
            await GetProjectTaskDocumentsAsync();
            await CloseCreateProjectTaskDocumentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProjectTaskDocumentModalAsync()
    {
        await EditProjectTaskDocumentModal.Hide();
    }

    private async Task UpdateProjectTaskDocumentAsync()
    {
        try
        {
            if (await EditingProjectTaskDocumentValidations.ValidateAll() == false)
            {
                return;
            }

            await ProjectTaskDocumentsAppService.UpdateAsync(EditingProjectTaskDocumentId, EditingProjectTaskDocument);
            await GetProjectTaskDocumentsAsync();
            await EditProjectTaskDocumentModal.Hide();
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

    protected virtual async Task OnDocumentPurposeChangedAsync(string? documentPurpose)
    {
        Filter.DocumentPurpose = documentPurpose;
        await SearchAsync();
    }

    protected virtual async Task OnProjectTaskIdChangedAsync(Guid? projectTaskId)
    {
        Filter.ProjectTaskId = projectTaskId;
        await SearchAsync();
    }

    protected virtual async Task OnDocumentIdChangedAsync(Guid? documentId)
    {
        Filter.DocumentId = documentId;
        await SearchAsync();
    }

    private async Task GetProjectTaskCollectionLookupAsync(string? newValue = null)
    {
        ProjectTasksCollection = (await ProjectTaskDocumentsAppService.GetProjectTaskLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetDocumentCollectionLookupAsync(string? newValue = null)
    {
        DocumentsCollection = (await ProjectTaskDocumentsAppService.GetDocumentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllProjectTaskDocumentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProjectTaskDocumentsSelected = false;
        SelectedProjectTaskDocuments.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedProjectTaskDocumentRowsChanged()
    {
        if (SelectedProjectTaskDocuments.Count != PageSize)
        {
            AllProjectTaskDocumentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProjectTaskDocumentsAsync()
    {
        var message = AllProjectTaskDocumentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProjectTaskDocuments.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProjectTaskDocumentsSelected)
        {
            await ProjectTaskDocumentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProjectTaskDocumentsAppService.DeleteByIdsAsync(SelectedProjectTaskDocuments.Select(x => x.ProjectTaskDocument.Id).ToList());
        }

        SelectedProjectTaskDocuments.Clear();
        AllProjectTaskDocumentsSelected = false;
        await GetProjectTaskDocumentsAsync();
    }
}