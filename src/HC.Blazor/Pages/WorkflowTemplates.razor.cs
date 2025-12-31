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
using HC.WorkflowTemplates;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class WorkflowTemplates
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<WorkflowTemplateWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<WorkflowTemplateWithNavigationPropertiesDto> WorkflowTemplateList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateWorkflowTemplate { get; set; }

    private bool CanEditWorkflowTemplate { get; set; }

    private bool CanDeleteWorkflowTemplate { get; set; }

    private WorkflowTemplateCreateDto NewWorkflowTemplate { get; set; }

    private Validations NewWorkflowTemplateValidations { get; set; } = new();
    private WorkflowTemplateUpdateDto EditingWorkflowTemplate { get; set; }

    private Validations EditingWorkflowTemplateValidations { get; set; } = new();
    private Guid EditingWorkflowTemplateId { get; set; }

    private Modal CreateWorkflowTemplateModal { get; set; } = new();
    private Modal EditWorkflowTemplateModal { get; set; } = new();
    private GetWorkflowTemplatesInput Filter { get; set; }

    private DataGridEntityActionsColumn<WorkflowTemplateWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "workflowTemplate-create-tab";
    protected string SelectedEditTab = "workflowTemplate-edit-tab";
    private WorkflowTemplateWithNavigationPropertiesDto? SelectedWorkflowTemplate;

    private IReadOnlyList<LookupDto<Guid>> WorkflowsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<WorkflowTemplateWithNavigationPropertiesDto> SelectedWorkflowTemplates { get; set; } = new();
    private bool AllWorkflowTemplatesSelected { get; set; }

    public WorkflowTemplates()
    {
        NewWorkflowTemplate = new WorkflowTemplateCreateDto();
        EditingWorkflowTemplate = new WorkflowTemplateUpdateDto();
        Filter = new GetWorkflowTemplatesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        WorkflowTemplateList = new List<WorkflowTemplateWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetWorkflowCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["WorkflowTemplates"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewWorkflowTemplate"], async () => {
            await OpenCreateWorkflowTemplateModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.WorkflowTemplates.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(WorkflowTemplateWithNavigationPropertiesDto workflowTemplate)
    {
        DataGridRef.ToggleDetailRow(workflowTemplate, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<WorkflowTemplateWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteWorkflowTemplate;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<WorkflowTemplateWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateWorkflowTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowTemplates.Create);
        CanEditWorkflowTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowTemplates.Edit);
        CanDeleteWorkflowTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowTemplates.Delete);
    }

    private async Task GetWorkflowTemplatesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await WorkflowTemplatesAppService.GetListAsync(Filter);
        WorkflowTemplateList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetWorkflowTemplatesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await WorkflowTemplatesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/workflow-templates/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&OutputFormat={HttpUtility.UrlEncode(Filter.OutputFormat)}&WorkflowId={Filter.WorkflowId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WorkflowTemplateWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetWorkflowTemplatesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateWorkflowTemplateModalAsync()
    {
        NewWorkflowTemplate = new WorkflowTemplateCreateDto
        {
            WorkflowId = WorkflowsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "workflowTemplate-create-tab";
        await NewWorkflowTemplateValidations.ClearAll();
        await CreateWorkflowTemplateModal.Show();
    }

    private async Task CloseCreateWorkflowTemplateModalAsync()
    {
        NewWorkflowTemplate = new WorkflowTemplateCreateDto
        {
            WorkflowId = WorkflowsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateWorkflowTemplateModal.Hide();
    }

    private async Task OpenEditWorkflowTemplateModalAsync(WorkflowTemplateWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "workflowTemplate-edit-tab";
        var workflowTemplate = await WorkflowTemplatesAppService.GetWithNavigationPropertiesAsync(input.WorkflowTemplate.Id);
        EditingWorkflowTemplateId = workflowTemplate.WorkflowTemplate.Id;
        EditingWorkflowTemplate = ObjectMapper.Map<WorkflowTemplateDto, WorkflowTemplateUpdateDto>(workflowTemplate.WorkflowTemplate);
        await EditingWorkflowTemplateValidations.ClearAll();
        await EditWorkflowTemplateModal.Show();
    }

    private async Task DeleteWorkflowTemplateAsync(WorkflowTemplateWithNavigationPropertiesDto input)
    {
        try
        {
            await WorkflowTemplatesAppService.DeleteAsync(input.WorkflowTemplate.Id);
            await GetWorkflowTemplatesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateWorkflowTemplateAsync()
    {
        try
        {
            if (await NewWorkflowTemplateValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowTemplatesAppService.CreateAsync(NewWorkflowTemplate);
            await GetWorkflowTemplatesAsync();
            await CloseCreateWorkflowTemplateModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditWorkflowTemplateModalAsync()
    {
        await EditWorkflowTemplateModal.Hide();
    }

    private async Task UpdateWorkflowTemplateAsync()
    {
        try
        {
            if (await EditingWorkflowTemplateValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowTemplatesAppService.UpdateAsync(EditingWorkflowTemplateId, EditingWorkflowTemplate);
            await GetWorkflowTemplatesAsync();
            await EditWorkflowTemplateModal.Hide();
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

    protected virtual async Task OnCodeChangedAsync(string? code)
    {
        Filter.Code = code;
        await SearchAsync();
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }

    protected virtual async Task OnOutputFormatChangedAsync(string? outputFormat)
    {
        Filter.OutputFormat = outputFormat;
        await SearchAsync();
    }

    protected virtual async Task OnWorkflowIdChangedAsync(Guid? workflowId)
    {
        Filter.WorkflowId = workflowId;
        await SearchAsync();
    }

    private async Task GetWorkflowCollectionLookupAsync(string? newValue = null)
    {
        WorkflowsCollection = (await WorkflowTemplatesAppService.GetWorkflowLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllWorkflowTemplatesSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllWorkflowTemplatesSelected = false;
        SelectedWorkflowTemplates.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedWorkflowTemplateRowsChanged()
    {
        if (SelectedWorkflowTemplates.Count != PageSize)
        {
            AllWorkflowTemplatesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedWorkflowTemplatesAsync()
    {
        var message = AllWorkflowTemplatesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedWorkflowTemplates.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllWorkflowTemplatesSelected)
        {
            await WorkflowTemplatesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await WorkflowTemplatesAppService.DeleteByIdsAsync(SelectedWorkflowTemplates.Select(x => x.WorkflowTemplate.Id).ToList());
        }

        SelectedWorkflowTemplates.Clear();
        AllWorkflowTemplatesSelected = false;
        await GetWorkflowTemplatesAsync();
    }
}