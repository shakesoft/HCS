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
using HC.WorkflowStepAssignments;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class WorkflowStepAssignments
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<WorkflowStepAssignmentWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<WorkflowStepAssignmentWithNavigationPropertiesDto> WorkflowStepAssignmentList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateWorkflowStepAssignment { get; set; }

    private bool CanEditWorkflowStepAssignment { get; set; }

    private bool CanDeleteWorkflowStepAssignment { get; set; }

    private WorkflowStepAssignmentCreateDto NewWorkflowStepAssignment { get; set; }

    private Validations NewWorkflowStepAssignmentValidations { get; set; } = new();
    private WorkflowStepAssignmentUpdateDto EditingWorkflowStepAssignment { get; set; }

    private Validations EditingWorkflowStepAssignmentValidations { get; set; } = new();
    private Guid EditingWorkflowStepAssignmentId { get; set; }

    private Modal CreateWorkflowStepAssignmentModal { get; set; } = new();
    private Modal EditWorkflowStepAssignmentModal { get; set; } = new();
    private GetWorkflowStepAssignmentsInput Filter { get; set; }

    private DataGridEntityActionsColumn<WorkflowStepAssignmentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "workflowStepAssignment-create-tab";
    protected string SelectedEditTab = "workflowStepAssignment-edit-tab";
    private WorkflowStepAssignmentWithNavigationPropertiesDto? SelectedWorkflowStepAssignment;

    private IReadOnlyList<LookupDto<Guid>> WorkflowsCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> WorkflowStepTemplatesCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> WorkflowTemplatesCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<WorkflowStepAssignmentWithNavigationPropertiesDto> SelectedWorkflowStepAssignments { get; set; } = new();
    private bool AllWorkflowStepAssignmentsSelected { get; set; }

    public WorkflowStepAssignments()
    {
        NewWorkflowStepAssignment = new WorkflowStepAssignmentCreateDto();
        EditingWorkflowStepAssignment = new WorkflowStepAssignmentUpdateDto();
        Filter = new GetWorkflowStepAssignmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        WorkflowStepAssignmentList = new List<WorkflowStepAssignmentWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetWorkflowCollectionLookupAsync();
        await GetWorkflowStepTemplateCollectionLookupAsync();
        await GetWorkflowTemplateCollectionLookupAsync();
        await GetIdentityUserCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["WorkflowStepAssignments"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewWorkflowStepAssignment"], async () => {
            await OpenCreateWorkflowStepAssignmentModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.WorkflowStepAssignments.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(WorkflowStepAssignmentWithNavigationPropertiesDto workflowStepAssignment)
    {
        DataGridRef.ToggleDetailRow(workflowStepAssignment, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<WorkflowStepAssignmentWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteWorkflowStepAssignment;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<WorkflowStepAssignmentWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateWorkflowStepAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepAssignments.Create);
        CanEditWorkflowStepAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepAssignments.Edit);
        CanDeleteWorkflowStepAssignment = await AuthorizationService.IsGrantedAsync(HCPermissions.WorkflowStepAssignments.Delete);
    }

    private async Task GetWorkflowStepAssignmentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await WorkflowStepAssignmentsAppService.GetListAsync(Filter);
        WorkflowStepAssignmentList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetWorkflowStepAssignmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await WorkflowStepAssignmentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/workflow-step-assignments/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&IsPrimary={Filter.IsPrimary}&IsActive={Filter.IsActive}&WorkflowId={Filter.WorkflowId}&StepId={Filter.StepId}&TemplateId={Filter.TemplateId}&DefaultUserId={Filter.DefaultUserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<WorkflowStepAssignmentWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetWorkflowStepAssignmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateWorkflowStepAssignmentModalAsync()
    {
        NewWorkflowStepAssignment = new WorkflowStepAssignmentCreateDto
        {
        };
        SelectedCreateTab = "workflowStepAssignment-create-tab";
        await NewWorkflowStepAssignmentValidations.ClearAll();
        await CreateWorkflowStepAssignmentModal.Show();
    }

    private async Task CloseCreateWorkflowStepAssignmentModalAsync()
    {
        NewWorkflowStepAssignment = new WorkflowStepAssignmentCreateDto
        {
        };
        await CreateWorkflowStepAssignmentModal.Hide();
    }

    private async Task OpenEditWorkflowStepAssignmentModalAsync(WorkflowStepAssignmentWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "workflowStepAssignment-edit-tab";
        var workflowStepAssignment = await WorkflowStepAssignmentsAppService.GetWithNavigationPropertiesAsync(input.WorkflowStepAssignment.Id);
        EditingWorkflowStepAssignmentId = workflowStepAssignment.WorkflowStepAssignment.Id;
        EditingWorkflowStepAssignment = ObjectMapper.Map<WorkflowStepAssignmentDto, WorkflowStepAssignmentUpdateDto>(workflowStepAssignment.WorkflowStepAssignment);
        await EditingWorkflowStepAssignmentValidations.ClearAll();
        await EditWorkflowStepAssignmentModal.Show();
    }

    private async Task DeleteWorkflowStepAssignmentAsync(WorkflowStepAssignmentWithNavigationPropertiesDto input)
    {
        try
        {
            await WorkflowStepAssignmentsAppService.DeleteAsync(input.WorkflowStepAssignment.Id);
            await GetWorkflowStepAssignmentsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateWorkflowStepAssignmentAsync()
    {
        try
        {
            if (await NewWorkflowStepAssignmentValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowStepAssignmentsAppService.CreateAsync(NewWorkflowStepAssignment);
            await GetWorkflowStepAssignmentsAsync();
            await CloseCreateWorkflowStepAssignmentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditWorkflowStepAssignmentModalAsync()
    {
        await EditWorkflowStepAssignmentModal.Hide();
    }

    private async Task UpdateWorkflowStepAssignmentAsync()
    {
        try
        {
            if (await EditingWorkflowStepAssignmentValidations.ValidateAll() == false)
            {
                return;
            }

            await WorkflowStepAssignmentsAppService.UpdateAsync(EditingWorkflowStepAssignmentId, EditingWorkflowStepAssignment);
            await GetWorkflowStepAssignmentsAsync();
            await EditWorkflowStepAssignmentModal.Hide();
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

    protected virtual async Task OnIsPrimaryChangedAsync(bool? isPrimary)
    {
        Filter.IsPrimary = isPrimary;
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

    protected virtual async Task OnStepIdChangedAsync(Guid? stepId)
    {
        Filter.StepId = stepId;
        await SearchAsync();
    }

    protected virtual async Task OnTemplateIdChangedAsync(Guid? templateId)
    {
        Filter.TemplateId = templateId;
        await SearchAsync();
    }

    protected virtual async Task OnDefaultUserIdChangedAsync(Guid? defaultUserId)
    {
        Filter.DefaultUserId = defaultUserId;
        await SearchAsync();
    }

    private async Task GetWorkflowCollectionLookupAsync(string? newValue = null)
    {
        WorkflowsCollection = (await WorkflowStepAssignmentsAppService.GetWorkflowLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetWorkflowStepTemplateCollectionLookupAsync(string? newValue = null)
    {
        WorkflowStepTemplatesCollection = (await WorkflowStepAssignmentsAppService.GetWorkflowStepTemplateLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetWorkflowTemplateCollectionLookupAsync(string? newValue = null)
    {
        WorkflowTemplatesCollection = (await WorkflowStepAssignmentsAppService.GetWorkflowTemplateLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await WorkflowStepAssignmentsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllWorkflowStepAssignmentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllWorkflowStepAssignmentsSelected = false;
        SelectedWorkflowStepAssignments.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedWorkflowStepAssignmentRowsChanged()
    {
        if (SelectedWorkflowStepAssignments.Count != PageSize)
        {
            AllWorkflowStepAssignmentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedWorkflowStepAssignmentsAsync()
    {
        var message = AllWorkflowStepAssignmentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedWorkflowStepAssignments.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllWorkflowStepAssignmentsSelected)
        {
            await WorkflowStepAssignmentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await WorkflowStepAssignmentsAppService.DeleteByIdsAsync(SelectedWorkflowStepAssignments.Select(x => x.WorkflowStepAssignment.Id).ToList());
        }

        SelectedWorkflowStepAssignments.Clear();
        AllWorkflowStepAssignmentsSelected = false;
        await GetWorkflowStepAssignmentsAsync();
    }
}