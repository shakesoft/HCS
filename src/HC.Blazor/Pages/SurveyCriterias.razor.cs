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
using HC.SurveyCriterias;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class SurveyCriterias
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<SurveyCriteriaWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<SurveyCriteriaWithNavigationPropertiesDto> SurveyCriteriaList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateSurveyCriteria { get; set; }

    private bool CanEditSurveyCriteria { get; set; }

    private bool CanDeleteSurveyCriteria { get; set; }

    private SurveyCriteriaCreateDto NewSurveyCriteria { get; set; }

    private Validations NewSurveyCriteriaValidations { get; set; } = new();
    private SurveyCriteriaUpdateDto EditingSurveyCriteria { get; set; }

    private Validations EditingSurveyCriteriaValidations { get; set; } = new();
    private Guid EditingSurveyCriteriaId { get; set; }

    private Modal CreateSurveyCriteriaModal { get; set; } = new();
    private Modal EditSurveyCriteriaModal { get; set; } = new();
    private GetSurveyCriteriasInput Filter { get; set; }

    private DataGridEntityActionsColumn<SurveyCriteriaWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "surveyCriteria-create-tab";
    protected string SelectedEditTab = "surveyCriteria-edit-tab";
    private SurveyCriteriaWithNavigationPropertiesDto? SelectedSurveyCriteria;

    private IReadOnlyList<LookupDto<Guid>> SurveyLocationsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<SurveyCriteriaWithNavigationPropertiesDto> SelectedSurveyCriterias { get; set; } = new();
    private bool AllSurveyCriteriasSelected { get; set; }

    public SurveyCriterias()
    {
        NewSurveyCriteria = new SurveyCriteriaCreateDto();
        EditingSurveyCriteria = new SurveyCriteriaUpdateDto();
        Filter = new GetSurveyCriteriasInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        SurveyCriteriaList = new List<SurveyCriteriaWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetSurveyLocationCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SurveyCriterias"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewSurveyCriteria"], async () => {
            await OpenCreateSurveyCriteriaModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.SurveyCriterias.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(SurveyCriteriaWithNavigationPropertiesDto surveyCriteria)
    {
        DataGridRef.ToggleDetailRow(surveyCriteria, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<SurveyCriteriaWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteSurveyCriteria;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<SurveyCriteriaWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateSurveyCriteria = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyCriterias.Create);
        CanEditSurveyCriteria = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyCriterias.Edit);
        CanDeleteSurveyCriteria = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyCriterias.Delete);
    }

    private async Task GetSurveyCriteriasAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await SurveyCriteriasAppService.GetListAsync(Filter);
        SurveyCriteriaList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetSurveyCriteriasAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await SurveyCriteriasAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/survey-criterias/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Image={HttpUtility.UrlEncode(Filter.Image)}&DisplayOrderMin={Filter.DisplayOrderMin}&DisplayOrderMax={Filter.DisplayOrderMax}&IsActive={Filter.IsActive}&SurveyLocationId={Filter.SurveyLocationId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SurveyCriteriaWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetSurveyCriteriasAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateSurveyCriteriaModalAsync()
    {
        NewSurveyCriteria = new SurveyCriteriaCreateDto
        {
            SurveyLocationId = SurveyLocationsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "surveyCriteria-create-tab";
        await NewSurveyCriteriaValidations.ClearAll();
        await CreateSurveyCriteriaModal.Show();
    }

    private async Task CloseCreateSurveyCriteriaModalAsync()
    {
        NewSurveyCriteria = new SurveyCriteriaCreateDto
        {
            SurveyLocationId = SurveyLocationsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateSurveyCriteriaModal.Hide();
    }

    private async Task OpenEditSurveyCriteriaModalAsync(SurveyCriteriaWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "surveyCriteria-edit-tab";
        var surveyCriteria = await SurveyCriteriasAppService.GetWithNavigationPropertiesAsync(input.SurveyCriteria.Id);
        EditingSurveyCriteriaId = surveyCriteria.SurveyCriteria.Id;
        EditingSurveyCriteria = ObjectMapper.Map<SurveyCriteriaDto, SurveyCriteriaUpdateDto>(surveyCriteria.SurveyCriteria);
        await EditingSurveyCriteriaValidations.ClearAll();
        await EditSurveyCriteriaModal.Show();
    }

    private async Task DeleteSurveyCriteriaAsync(SurveyCriteriaWithNavigationPropertiesDto input)
    {
        try
        {
            await SurveyCriteriasAppService.DeleteAsync(input.SurveyCriteria.Id);
            await GetSurveyCriteriasAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateSurveyCriteriaAsync()
    {
        try
        {
            if (await NewSurveyCriteriaValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyCriteriasAppService.CreateAsync(NewSurveyCriteria);
            await GetSurveyCriteriasAsync();
            await CloseCreateSurveyCriteriaModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditSurveyCriteriaModalAsync()
    {
        await EditSurveyCriteriaModal.Hide();
    }

    private async Task UpdateSurveyCriteriaAsync()
    {
        try
        {
            if (await EditingSurveyCriteriaValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyCriteriasAppService.UpdateAsync(EditingSurveyCriteriaId, EditingSurveyCriteria);
            await GetSurveyCriteriasAsync();
            await EditSurveyCriteriaModal.Hide();
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

    protected virtual async Task OnImageChangedAsync(string? image)
    {
        Filter.Image = image;
        await SearchAsync();
    }

    protected virtual async Task OnDisplayOrderMinChangedAsync(int? displayOrderMin)
    {
        Filter.DisplayOrderMin = displayOrderMin;
        await SearchAsync();
    }

    protected virtual async Task OnDisplayOrderMaxChangedAsync(int? displayOrderMax)
    {
        Filter.DisplayOrderMax = displayOrderMax;
        await SearchAsync();
    }

    protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
    {
        Filter.IsActive = isActive;
        await SearchAsync();
    }

    protected virtual async Task OnSurveyLocationIdChangedAsync(Guid? surveyLocationId)
    {
        Filter.SurveyLocationId = surveyLocationId;
        await SearchAsync();
    }

    private async Task GetSurveyLocationCollectionLookupAsync(string? newValue = null)
    {
        SurveyLocationsCollection = (await SurveyCriteriasAppService.GetSurveyLocationLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllSurveyCriteriasSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllSurveyCriteriasSelected = false;
        SelectedSurveyCriterias.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedSurveyCriteriaRowsChanged()
    {
        if (SelectedSurveyCriterias.Count != PageSize)
        {
            AllSurveyCriteriasSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedSurveyCriteriasAsync()
    {
        var message = AllSurveyCriteriasSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSurveyCriterias.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllSurveyCriteriasSelected)
        {
            await SurveyCriteriasAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await SurveyCriteriasAppService.DeleteByIdsAsync(SelectedSurveyCriterias.Select(x => x.SurveyCriteria.Id).ToList());
        }

        SelectedSurveyCriterias.Clear();
        AllSurveyCriteriasSelected = false;
        await GetSurveyCriteriasAsync();
    }
}