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
using HC.Departments;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using System.Threading;

namespace HC.Blazor.Pages;

public partial class DepartmentsBackup
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<DepartmentWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<DepartmentWithNavigationPropertiesDto> DepartmentList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateDepartment { get; set; }

    private bool CanEditDepartment { get; set; }

    private bool CanDeleteDepartment { get; set; }

    private DepartmentCreateDto NewDepartment { get; set; }

    private Validations NewDepartmentValidations { get; set; } = new();
    private DepartmentUpdateDto EditingDepartment { get; set; }

    private Validations EditingDepartmentValidations { get; set; } = new();
    private Guid EditingDepartmentId { get; set; }

    private Modal CreateDepartmentModal { get; set; } = new();
    private Modal EditDepartmentModal { get; set; } = new();
    private GetDepartmentsInput Filter { get; set; }

    private DataGridEntityActionsColumn<DepartmentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "department-create-tab";
    protected string SelectedEditTab = "department-edit-tab";
    private DepartmentWithNavigationPropertiesDto? SelectedDepartment;

    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<DepartmentWithNavigationPropertiesDto> SelectedDepartments { get; set; } = new();
    private bool AllDepartmentsSelected { get; set; }


    private List<LookupDto<Guid>> SelectedLeaderUser { get; set; } = new();
    public DepartmentsBackup()
    {
        NewDepartment = new DepartmentCreateDto();
        EditingDepartment = new DepartmentUpdateDto();
        Filter = new GetDepartmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        DepartmentList = new List<DepartmentWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Departments"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewDepartment"], async () => {
            await OpenCreateDepartmentModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.Departments.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(DepartmentWithNavigationPropertiesDto department)
    {
        DataGridRef.ToggleDetailRow(department, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<DepartmentWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteDepartment;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<DepartmentWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDepartment = await AuthorizationService.IsGrantedAsync(HCPermissions.Departments.Create);
        CanEditDepartment = await AuthorizationService.IsGrantedAsync(HCPermissions.Departments.Edit);
        CanDeleteDepartment = await AuthorizationService.IsGrantedAsync(HCPermissions.Departments.Delete);
    }

    private async Task GetDepartmentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await DepartmentsAppService.GetListAsync(Filter);
        DepartmentList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetDepartmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await DepartmentsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/departments/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&ParentId={HttpUtility.UrlEncode(Filter.ParentId)}&LevelMin={Filter.LevelMin}&LevelMax={Filter.LevelMax}&SortOrderMin={Filter.SortOrderMin}&SortOrderMax={Filter.SortOrderMax}&IsActive={Filter.IsActive}&LeaderUserId={Filter.LeaderUserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DepartmentWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetDepartmentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateDepartmentModalAsync()
    {
        NewDepartment = new DepartmentCreateDto
        {
        };
        SelectedCreateTab = "department-create-tab";
        await NewDepartmentValidations.ClearAll();
        await CreateDepartmentModal.Show();
    }

    private async Task CloseCreateDepartmentModalAsync()
    {
        NewDepartment = new DepartmentCreateDto
        {
        };
        await CreateDepartmentModal.Hide();
    }

    private async Task OpenEditDepartmentModalAsync(DepartmentWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "department-edit-tab";
        var department = await DepartmentsAppService.GetWithNavigationPropertiesAsync(input.Department.Id);
        EditingDepartmentId = department.Department.Id;
        EditingDepartment = ObjectMapper.Map<DepartmentDto, DepartmentUpdateDto>(department.Department);
        await EditingDepartmentValidations.ClearAll();
        await EditDepartmentModal.Show();
    }

    private async Task DeleteDepartmentAsync(DepartmentWithNavigationPropertiesDto input)
    {
        try
        {
            await DepartmentsAppService.DeleteAsync(input.Department.Id);
            await GetDepartmentsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteDepartmentWithConfirmationAsync(DepartmentWithNavigationPropertiesDto input)
    {
        if (await UiMessageService.Confirm(L["DeleteConfirmationMessage"].Value))
        {
            await DeleteDepartmentAsync(input);
        }
    }

    private async Task CreateDepartmentAsync()
    {
        try
        {
            if (await NewDepartmentValidations.ValidateAll() == false)
            {
                return;
            }

            await DepartmentsAppService.CreateAsync(NewDepartment);
            await GetDepartmentsAsync();
            await CloseCreateDepartmentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditDepartmentModalAsync()
    {
        await EditDepartmentModal.Hide();
    }

    private async Task UpdateDepartmentAsync()
    {
        try
        {
            if (await EditingDepartmentValidations.ValidateAll() == false)
            {
                return;
            }

            await DepartmentsAppService.UpdateAsync(EditingDepartmentId, EditingDepartment);
            await GetDepartmentsAsync();
            await EditDepartmentModal.Hide();
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

    protected virtual async Task OnParentIdChangedAsync(string? parentId)
    {
        Filter.ParentId = parentId;
        await SearchAsync();
    }

    protected virtual async Task OnLevelMinChangedAsync(int? levelMin)
    {
        Filter.LevelMin = levelMin;
        await SearchAsync();
    }

    protected virtual async Task OnLevelMaxChangedAsync(int? levelMax)
    {
        Filter.LevelMax = levelMax;
        await SearchAsync();
    }

    protected virtual async Task OnSortOrderMinChangedAsync(int? sortOrderMin)
    {
        Filter.SortOrderMin = sortOrderMin;
        await SearchAsync();
    }

    protected virtual async Task OnSortOrderMaxChangedAsync(int? sortOrderMax)
    {
        Filter.SortOrderMax = sortOrderMax;
        await SearchAsync();
    }

    protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
    {
        Filter.IsActive = isActive;
        await SearchAsync();
    }

    protected virtual void OnLeaderUserIdChanged()
    {
        NewDepartment.LeaderUserId = SelectedLeaderUser.FirstOrDefault()?.Id ?? Guid.Empty;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await DepartmentsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task<List<LookupDto<Guid>>> GetIdentityUserCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        IdentityUsersCollection = (await DepartmentsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = filter })).Items;
        return IdentityUsersCollection.ToList();
    }


    private Task SelectAllItems()
    {
        AllDepartmentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllDepartmentsSelected = false;
        SelectedDepartments.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedDepartmentRowsChanged()
    {
        if (SelectedDepartments.Count != PageSize)
        {
            AllDepartmentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedDepartmentsAsync()
    {
        var message = AllDepartmentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedDepartments.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllDepartmentsSelected)
        {
            await DepartmentsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await DepartmentsAppService.DeleteByIdsAsync(SelectedDepartments.Select(x => x.Department.Id).ToList());
        }

        SelectedDepartments.Clear();
        AllDepartmentsSelected = false;
        await GetDepartmentsAsync();
    }
}