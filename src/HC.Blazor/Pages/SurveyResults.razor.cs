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
using HC.SurveyResults;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class SurveyResults
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<SurveyResultWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<SurveyResultWithNavigationPropertiesDto> SurveyResultList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateSurveyResult { get; set; }

    private bool CanEditSurveyResult { get; set; }

    private bool CanDeleteSurveyResult { get; set; }

    private SurveyResultCreateDto NewSurveyResult { get; set; }

    private Validations NewSurveyResultValidations { get; set; } = new();
    private SurveyResultUpdateDto EditingSurveyResult { get; set; }

    private Validations EditingSurveyResultValidations { get; set; } = new();
    private Guid EditingSurveyResultId { get; set; }

    private Modal CreateSurveyResultModal { get; set; } = new();
    private Modal EditSurveyResultModal { get; set; } = new();
    private GetSurveyResultsInput Filter { get; set; }

    private DataGridEntityActionsColumn<SurveyResultWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "surveyResult-create-tab";
    protected string SelectedEditTab = "surveyResult-edit-tab";
    private SurveyResultWithNavigationPropertiesDto? SelectedSurveyResult;

    private IReadOnlyList<LookupDto<Guid>> SurveyCriteriasCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> SurveySessionsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<SurveyResultWithNavigationPropertiesDto> SelectedSurveyResults { get; set; } = new();
    private bool AllSurveyResultsSelected { get; set; }

    public SurveyResults()
    {
        NewSurveyResult = new SurveyResultCreateDto();
        EditingSurveyResult = new SurveyResultUpdateDto();
        Filter = new GetSurveyResultsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        SurveyResultList = new List<SurveyResultWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetSurveyCriteriaCollectionLookupAsync();
        await GetSurveySessionCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SurveyResults"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewSurveyResult"], async () => {
            await OpenCreateSurveyResultModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.SurveyResults.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(SurveyResultWithNavigationPropertiesDto surveyResult)
    {
        DataGridRef.ToggleDetailRow(surveyResult, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<SurveyResultWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteSurveyResult;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<SurveyResultWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateSurveyResult = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyResults.Create);
        CanEditSurveyResult = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyResults.Edit);
        CanDeleteSurveyResult = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyResults.Delete);
    }

    private async Task GetSurveyResultsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await SurveyResultsAppService.GetListAsync(Filter);
        SurveyResultList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetSurveyResultsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await SurveyResultsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/survey-results/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&RatingMin={Filter.RatingMin}&RatingMax={Filter.RatingMax}&SurveyCriteriaId={Filter.SurveyCriteriaId}&SurveySessionId={Filter.SurveySessionId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SurveyResultWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetSurveyResultsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateSurveyResultModalAsync()
    {
        NewSurveyResult = new SurveyResultCreateDto
        {
            SurveyCriteriaId = SurveyCriteriasCollection.Select(i => i.Id).FirstOrDefault(),
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "surveyResult-create-tab";
        await NewSurveyResultValidations.ClearAll();
        await CreateSurveyResultModal.Show();
    }

    private async Task CloseCreateSurveyResultModalAsync()
    {
        NewSurveyResult = new SurveyResultCreateDto
        {
            SurveyCriteriaId = SurveyCriteriasCollection.Select(i => i.Id).FirstOrDefault(),
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateSurveyResultModal.Hide();
    }

    private async Task OpenEditSurveyResultModalAsync(SurveyResultWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "surveyResult-edit-tab";
        var surveyResult = await SurveyResultsAppService.GetWithNavigationPropertiesAsync(input.SurveyResult.Id);
        EditingSurveyResultId = surveyResult.SurveyResult.Id;
        EditingSurveyResult = ObjectMapper.Map<SurveyResultDto, SurveyResultUpdateDto>(surveyResult.SurveyResult);
        await EditingSurveyResultValidations.ClearAll();
        await EditSurveyResultModal.Show();
    }

    private async Task DeleteSurveyResultAsync(SurveyResultWithNavigationPropertiesDto input)
    {
        try
        {
            await SurveyResultsAppService.DeleteAsync(input.SurveyResult.Id);
            await GetSurveyResultsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateSurveyResultAsync()
    {
        try
        {
            if (await NewSurveyResultValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyResultsAppService.CreateAsync(NewSurveyResult);
            await GetSurveyResultsAsync();
            await CloseCreateSurveyResultModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditSurveyResultModalAsync()
    {
        await EditSurveyResultModal.Hide();
    }

    private async Task UpdateSurveyResultAsync()
    {
        try
        {
            if (await EditingSurveyResultValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyResultsAppService.UpdateAsync(EditingSurveyResultId, EditingSurveyResult);
            await GetSurveyResultsAsync();
            await EditSurveyResultModal.Hide();
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

    protected virtual async Task OnRatingMinChangedAsync(int? ratingMin)
    {
        Filter.RatingMin = ratingMin;
        await SearchAsync();
    }

    protected virtual async Task OnRatingMaxChangedAsync(int? ratingMax)
    {
        Filter.RatingMax = ratingMax;
        await SearchAsync();
    }

    protected virtual async Task OnSurveyCriteriaIdChangedAsync(Guid? surveyCriteriaId)
    {
        Filter.SurveyCriteriaId = surveyCriteriaId;
        await SearchAsync();
    }

    protected virtual async Task OnSurveySessionIdChangedAsync(Guid? surveySessionId)
    {
        Filter.SurveySessionId = surveySessionId;
        await SearchAsync();
    }

    private async Task GetSurveyCriteriaCollectionLookupAsync(string? newValue = null)
    {
        SurveyCriteriasCollection = (await SurveyResultsAppService.GetSurveyCriteriaLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetSurveySessionCollectionLookupAsync(string? newValue = null)
    {
        SurveySessionsCollection = (await SurveyResultsAppService.GetSurveySessionLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllSurveyResultsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllSurveyResultsSelected = false;
        SelectedSurveyResults.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedSurveyResultRowsChanged()
    {
        if (SelectedSurveyResults.Count != PageSize)
        {
            AllSurveyResultsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedSurveyResultsAsync()
    {
        var message = AllSurveyResultsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSurveyResults.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllSurveyResultsSelected)
        {
            await SurveyResultsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await SurveyResultsAppService.DeleteByIdsAsync(SelectedSurveyResults.Select(x => x.SurveyResult.Id).ToList());
        }

        SelectedSurveyResults.Clear();
        AllSurveyResultsSelected = false;
        await GetSurveyResultsAsync();
    }
}