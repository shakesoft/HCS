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
using HC.WorkflowStepTemplates;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class WorkflowStepTemplates
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<WorkflowStepTemplateWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<WorkflowStepTemplateWithNavigationPropertiesDto> WorkflowStepTemplateList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateWorkflowStepTemplate { get; set; }

    private bool CanEditWorkflowStepTemplate { get; set; }

    private bool CanDeleteWorkflowStepTemplate { get; set; }

    private WorkflowStepTemplateCreateDto NewWorkflowStepTemplate { get; set; }

    private Validations NewWorkflowStepTemplateValidations { get; set; } = new();
    private WorkflowStepTemplateUpdateDto EditingWorkflowStepTemplate { get; set; }

    private Validations EditingWorkflowStepTemplateValidations { get; set; } = new();
    private Guid EditingWorkflowStepTemplateId { get; set; }

    private Modal CreateWorkflowStepTemplateModal { get; set; } = new();
    private Modal EditWorkflowStepTemplateModal { get; set; } = new();
    private GetWorkflowStepTemplatesInput Filter { get; set; }

    private DataGridEntityActionsColumn<WorkflowStepTemplateWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "workflowStepTemplate-create-tab";
    protected string SelectedEditTab = "workflowStepTemplate-edit-tab";
    private WorkflowStepTemplateWithNavigationPropertiesDto? SelectedWorkflowStepTemplate;

    private IReadOnlyList<LookupDto<Guid>> WorkflowsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<WorkflowStepTemplateWithNavigationPropertiesDto> SelectedWorkflowStepTemplates { get; set; } = new();
    private bool AllWorkflowStepTemplatesSelected { get; set; }

    public WorkflowStepTemplates()
    {
        NewWorkflowStepTemplate = new WorkflowStepTemplateCreateDto();
        EditingWorkflowStepTemplate = new WorkflowStepTemplateUpdateDto();
        Filter = new GetWorkflowStepTemplatesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        WorkflowStepTemplateList = new List<WorkflowStepTemplateWithNavigationPropertiesDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["WorkflowStepTemplates"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewWorkflowStepTemplate"], async () => {
            await OpenCreateWorkflowStepTemplateModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.WorkflowStepTemplates.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(WorkflowStepTemplateWithNavigationPropertiesDto workflowStepTemplate)
    {
        DataGridRef.ToggleDetailRow(workflowStepTemplate, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<WorkflowStepTemplateWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteWorkflowStepTemplate;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<WorkflowStepTemplateWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateWorkflowStepTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepTemplates.Create);
        CanEditWorkflowStepTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepTemplates.Edit);
        CanDeleteWorkflowStepTemplate = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepTemplates.Delete);
    }

    private async Task GetWorkflowStepTemplatesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await WorkflowStepTemplatesAppService.GetListAsync(Filter);
        WorkflowStepTemplateList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetWorkflowStepTemplatesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await WorkflowStepTemplatesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/workflow-step-templates/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&Name={HttpUtility.UrlEncode(Filter.Name)}&Type={HttpUtility.UrlEncode(Filter.Type)}&SLADaysMin={Filter.SLADaysMin}&SLADaysMax={Filter.SLADaysMax}&IsActive={Filter.IsActive}&WorkflowId={Filter.WorkflowId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WorkflowStepTemplateWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetWorkflowStepTemplatesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateWorkflowStepTemplateModalAsync()
    {
        NewWorkflowStepTemplate = new WorkflowStepTemplateCreateDto
        {
            WorkflowId = WorkflowsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "workflowStepTemplate-create-tab";
        await NewWorkflowStepTemplateValidations.ClearAll();
        await CreateWorkflowStepTemplateModal.Show();
    }

    private async Task CloseCreateWorkflowStepTemplateModalAsync()
    {
        NewWorkflowStepTemplate = new WorkflowStepTemplateCreateDto
        {
            WorkflowId = WorkflowsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateWorkflowStepTemplateModal.Hide();
    }

    private async Task OpenEditWorkflowStepTemplateModalAsync(WorkflowStepTemplateWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "workflowStepTemplate-edit-tab";
        var workflowStepTemplate = await WorkflowStepTemplatesAppService.GetWithNavigationPropertiesAsync(input.WorkflowStepTemplate.Id);
        EditingWorkflowStepTemplateId = workflowStepTemplate.WorkflowStepTemplate.Id;
        EditingWorkflowStepTemplate = ObjectMapper.Map<WorkflowStepTemplateDto, WorkflowStepTemplateUpdateDto>(workflowStepTemplate.WorkflowStepTemplate);
        await EditingWorkflowStepTemplateValidations.ClearAll();
        await EditWorkflowStepTemplateModal.Show();
    }

    private async Task DeleteWorkflowStepTemplateAsync(WorkflowStepTemplateWithNavigationPropertiesDto input)
    {
        try
        {
            await WorkflowStepTemplatesAppService.DeleteAsync(input.WorkflowStepTemplate.Id);
            await GetWorkflowStepTemplatesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateWorkflowStepTemplateAsync()
    {
        try
        {
            if (await NewWorkflowStepTemplateValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowStepTemplatesAppService.CreateAsync(NewWorkflowStepTemplate);
            await GetWorkflowStepTemplatesAsync();
            await CloseCreateWorkflowStepTemplateModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditWorkflowStepTemplateModalAsync()
    {
        await EditWorkflowStepTemplateModal.Hide();
    }

    private async Task UpdateWorkflowStepTemplateAsync()
    {
        try
        {
            if (await EditingWorkflowStepTemplateValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowStepTemplatesAppService.UpdateAsync(EditingWorkflowStepTemplateId, EditingWorkflowStepTemplate);
            await GetWorkflowStepTemplatesAsync();
            await EditWorkflowStepTemplateModal.Hide();
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

    protected virtual async Task OnOrderMinChangedAsync(int? orderMin)
    {
        Filter.OrderMin = orderMin;
        await SearchAsync();
    }

    protected virtual async Task OnOrderMaxChangedAsync(int? orderMax)
    {
        Filter.OrderMax = orderMax;
        await SearchAsync();
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }

    protected virtual async Task OnTypeChangedAsync(string? type)
    {
        Filter.Type = type;
        await SearchAsync();
    }

    protected virtual async Task OnSLADaysMinChangedAsync(int? sLADaysMin)
    {
        Filter.SLADaysMin = sLADaysMin;
        await SearchAsync();
    }

    protected virtual async Task OnSLADaysMaxChangedAsync(int? sLADaysMax)
    {
        Filter.SLADaysMax = sLADaysMax;
        await SearchAsync();
    }

    protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
    {
        Filter.IsActive = isActive;
        await SearchAsync();
    }

    protected virtual async Task OnWorkflowIdChangedAsync(Guid? workflowId)
    {
        Filter.WorkflowId = workflowId;
        await SearchAsync();
    }

    private async Task GetWorkflowCollectionLookupAsync(string? newValue = null)
    {
        WorkflowsCollection = (await WorkflowStepTemplatesAppService.GetWorkflowLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllWorkflowStepTemplatesSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllWorkflowStepTemplatesSelected = false;
        SelectedWorkflowStepTemplates.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedWorkflowStepTemplateRowsChanged()
    {
        if (SelectedWorkflowStepTemplates.Count != PageSize)
        {
            AllWorkflowStepTemplatesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedWorkflowStepTemplatesAsync()
    {
        var message = AllWorkflowStepTemplatesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedWorkflowStepTemplates.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllWorkflowStepTemplatesSelected)
        {
            await WorkflowStepTemplatesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await WorkflowStepTemplatesAppService.DeleteByIdsAsync(SelectedWorkflowStepTemplates.Select(x => x.WorkflowStepTemplate.Id).ToList());
        }

        SelectedWorkflowStepTemplates.Clear();
        AllWorkflowStepTemplatesSelected = false;
        await GetWorkflowStepTemplatesAsync();
    }
}