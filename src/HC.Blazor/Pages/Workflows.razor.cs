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
using HC.Workflows;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class Workflows
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<WorkflowDto> DataGridRef { get; set; }

    private IReadOnlyList<WorkflowDto> WorkflowList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateWorkflow { get; set; }

    private bool CanEditWorkflow { get; set; }

    private bool CanDeleteWorkflow { get; set; }

    private WorkflowCreateDto NewWorkflow { get; set; }

    private Validations NewWorkflowValidations { get; set; } = new();
    private WorkflowUpdateDto EditingWorkflow { get; set; }

    private Validations EditingWorkflowValidations { get; set; } = new();
    private Guid EditingWorkflowId { get; set; }

    private Modal CreateWorkflowModal { get; set; } = new();
    private Modal EditWorkflowModal { get; set; } = new();
    private GetWorkflowsInput Filter { get; set; }

    private DataGridEntityActionsColumn<WorkflowDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "workflow-create-tab";
    protected string SelectedEditTab = "workflow-edit-tab";
    private WorkflowDto? SelectedWorkflow;

    private List<WorkflowDto> SelectedWorkflows { get; set; } = new();
    private bool AllWorkflowsSelected { get; set; }

    public Workflows()
    {
        NewWorkflow = new WorkflowCreateDto();
        EditingWorkflow = new WorkflowUpdateDto();
        Filter = new GetWorkflowsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        WorkflowList = new List<WorkflowDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Workflows"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewWorkflow"], async () => {
            await OpenCreateWorkflowModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.Workflows.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(WorkflowDto workflow)
    {
        DataGridRef.ToggleDetailRow(workflow, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<WorkflowDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteWorkflow;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<WorkflowDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateWorkflow = await AuthorizationService.IsGrantedAsync(HCPermissions.Workflows.Create);
        CanEditWorkflow = await AuthorizationService.IsGrantedAsync(HCPermissions.Workflows.Edit);
        CanDeleteWorkflow = await AuthorizationService.IsGrantedAsync(HCPermissions.Workflows.Delete);
    }

    private async Task GetWorkflowsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await WorkflowsAppService.GetListAsync(Filter);
        WorkflowList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetWorkflowsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await WorkflowsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/workflows/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Description={HttpUtility.UrlEncode(Filter.Description)}&IsActive={Filter.IsActive}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WorkflowDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetWorkflowsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateWorkflowModalAsync()
    {
        NewWorkflow = new WorkflowCreateDto
        {
        };
        SelectedCreateTab = "workflow-create-tab";
        await NewWorkflowValidations.ClearAll();
        await CreateWorkflowModal.Show();
    }

    private async Task CloseCreateWorkflowModalAsync()
    {
        NewWorkflow = new WorkflowCreateDto
        {
        };
        await CreateWorkflowModal.Hide();
    }

    private async Task OpenEditWorkflowModalAsync(WorkflowDto input)
    {
        SelectedEditTab = "workflow-edit-tab";
        var workflow = await WorkflowsAppService.GetAsync(input.Id);
        EditingWorkflowId = workflow.Id;
        EditingWorkflow = ObjectMapper.Map<WorkflowDto, WorkflowUpdateDto>(workflow);
        await EditingWorkflowValidations.ClearAll();
        await EditWorkflowModal.Show();
    }

    private async Task DeleteWorkflowAsync(WorkflowDto input)
    {
        try
        {
            await WorkflowsAppService.DeleteAsync(input.Id);
            await GetWorkflowsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateWorkflowAsync()
    {
        try
        {
            if (await NewWorkflowValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowsAppService.CreateAsync(NewWorkflow);
            await GetWorkflowsAsync();
            await CloseCreateWorkflowModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditWorkflowModalAsync()
    {
        await EditWorkflowModal.Hide();
    }

    private async Task UpdateWorkflowAsync()
    {
        try
        {
            if (await EditingWorkflowValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowsAppService.UpdateAsync(EditingWorkflowId, EditingWorkflow);
            await GetWorkflowsAsync();
            await EditWorkflowModal.Hide();
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

    protected virtual async Task OnDescriptionChangedAsync(string? description)
    {
        Filter.Description = description;
        await SearchAsync();
    }

    protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
    {
        Filter.IsActive = isActive;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllWorkflowsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllWorkflowsSelected = false;
        SelectedWorkflows.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedWorkflowRowsChanged()
    {
        if (SelectedWorkflows.Count != PageSize)
        {
            AllWorkflowsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedWorkflowsAsync()
    {
        var message = AllWorkflowsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedWorkflows.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllWorkflowsSelected)
        {
            await WorkflowsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await WorkflowsAppService.DeleteByIdsAsync(SelectedWorkflows.Select(x => x.Id).ToList());
        }

        SelectedWorkflows.Clear();
        AllWorkflowsSelected = false;
        await GetWorkflowsAsync();
    }
}