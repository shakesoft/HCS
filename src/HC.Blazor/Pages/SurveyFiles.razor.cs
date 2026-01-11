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
using HC.SurveyFiles;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class SurveyFiles
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<SurveyFileWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<SurveyFileWithNavigationPropertiesDto> SurveyFileList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateSurveyFile { get; set; }

    private bool CanEditSurveyFile { get; set; }

    private bool CanDeleteSurveyFile { get; set; }

    private SurveyFileCreateDto NewSurveyFile { get; set; }

    private Validations NewSurveyFileValidations { get; set; } = new();
    private SurveyFileUpdateDto EditingSurveyFile { get; set; }

    private Validations EditingSurveyFileValidations { get; set; } = new();
    private Guid EditingSurveyFileId { get; set; }

    private Modal CreateSurveyFileModal { get; set; } = new();
    private Modal EditSurveyFileModal { get; set; } = new();
    private GetSurveyFilesInput Filter { get; set; }

    private DataGridEntityActionsColumn<SurveyFileWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "surveyFile-create-tab";
    protected string SelectedEditTab = "surveyFile-edit-tab";
    private SurveyFileWithNavigationPropertiesDto? SelectedSurveyFile;

    private IReadOnlyList<LookupDto<Guid>> SurveySessionsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<SurveyFileWithNavigationPropertiesDto> SelectedSurveyFiles { get; set; } = new();
    private bool AllSurveyFilesSelected { get; set; }

    public SurveyFiles()
    {
        NewSurveyFile = new SurveyFileCreateDto();
        EditingSurveyFile = new SurveyFileUpdateDto();
        Filter = new GetSurveyFilesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        SurveyFileList = new List<SurveyFileWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SurveyFiles"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewSurveyFile"], async () => {
            await OpenCreateSurveyFileModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.SurveyFiles.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(SurveyFileWithNavigationPropertiesDto surveyFile)
    {
        DataGridRef.ToggleDetailRow(surveyFile, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<SurveyFileWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteSurveyFile;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<SurveyFileWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Create);
        CanEditSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Edit);
        CanDeleteSurveyFile = await AuthorizationService.IsGrantedAsync(HCPermissions.SurveyFiles.Delete);
    }

    private async Task GetSurveyFilesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await SurveyFilesAppService.GetListAsync(Filter);
        SurveyFileList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetSurveyFilesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await SurveyFilesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/survey-files/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&UploaderType={HttpUtility.UrlEncode(Filter.UploaderType)}&FileName={HttpUtility.UrlEncode(Filter.FileName)}&FilePath={HttpUtility.UrlEncode(Filter.FilePath)}&FileSizeMin={Filter.FileSizeMin}&FileSizeMax={Filter.FileSizeMax}&MimeType={HttpUtility.UrlEncode(Filter.MimeType)}&FileType={HttpUtility.UrlEncode(Filter.FileType)}&SurveySessionId={Filter.SurveySessionId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SurveyFileWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetSurveyFilesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateSurveyFileModalAsync()
    {
        NewSurveyFile = new SurveyFileCreateDto
        {
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "surveyFile-create-tab";
        await NewSurveyFileValidations.ClearAll();
        await CreateSurveyFileModal.Show();
    }

    private async Task CloseCreateSurveyFileModalAsync()
    {
        NewSurveyFile = new SurveyFileCreateDto
        {
            SurveySessionId = SurveySessionsCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateSurveyFileModal.Hide();
    }

    private async Task OpenEditSurveyFileModalAsync(SurveyFileWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "surveyFile-edit-tab";
        var surveyFile = await SurveyFilesAppService.GetWithNavigationPropertiesAsync(input.SurveyFile.Id);
        EditingSurveyFileId = surveyFile.SurveyFile.Id;
        EditingSurveyFile = ObjectMapper.Map<SurveyFileDto, SurveyFileUpdateDto>(surveyFile.SurveyFile);
        await EditingSurveyFileValidations.ClearAll();
        await EditSurveyFileModal.Show();
    }

    private async Task DeleteSurveyFileAsync(SurveyFileWithNavigationPropertiesDto input)
    {
        try
        {
            await SurveyFilesAppService.DeleteAsync(input.SurveyFile.Id);
            await GetSurveyFilesAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateSurveyFileAsync()
    {
        try
        {
            if (await NewSurveyFileValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyFilesAppService.CreateAsync(NewSurveyFile);
            await GetSurveyFilesAsync();
            await CloseCreateSurveyFileModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditSurveyFileModalAsync()
    {
        await EditSurveyFileModal.Hide();
    }

    private async Task UpdateSurveyFileAsync()
    {
        try
        {
            if (await EditingSurveyFileValidations.ValidateAll() == false)
            {
                return;
            }

            await SurveyFilesAppService.UpdateAsync(EditingSurveyFileId, EditingSurveyFile);
            await GetSurveyFilesAsync();
            await EditSurveyFileModal.Hide();
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

    protected virtual async Task OnUploaderTypeChangedAsync(string? uploaderType)
    {
        Filter.UploaderType = uploaderType;
        await SearchAsync();
    }

    protected virtual async Task OnFileNameChangedAsync(string? fileName)
    {
        Filter.FileName = fileName;
        await SearchAsync();
    }

    protected virtual async Task OnFilePathChangedAsync(string? filePath)
    {
        Filter.FilePath = filePath;
        await SearchAsync();
    }

    protected virtual async Task OnFileSizeMinChangedAsync(int? fileSizeMin)
    {
        Filter.FileSizeMin = fileSizeMin;
        await SearchAsync();
    }

    protected virtual async Task OnFileSizeMaxChangedAsync(int? fileSizeMax)
    {
        Filter.FileSizeMax = fileSizeMax;
        await SearchAsync();
    }

    protected virtual async Task OnMimeTypeChangedAsync(string? mimeType)
    {
        Filter.MimeType = mimeType;
        await SearchAsync();
    }

    protected virtual async Task OnFileTypeChangedAsync(string? fileType)
    {
        Filter.FileType = fileType;
        await SearchAsync();
    }

    protected virtual async Task OnSurveySessionIdChangedAsync(Guid? surveySessionId)
    {
        Filter.SurveySessionId = surveySessionId;
        await SearchAsync();
    }

    private async Task GetSurveySessionCollectionLookupAsync(string? newValue = null)
    {
        SurveySessionsCollection = (await SurveyFilesAppService.GetSurveySessionLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllSurveyFilesSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllSurveyFilesSelected = false;
        SelectedSurveyFiles.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedSurveyFileRowsChanged()
    {
        if (SelectedSurveyFiles.Count != PageSize)
        {
            AllSurveyFilesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedSurveyFilesAsync()
    {
        var message = AllSurveyFilesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSurveyFiles.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllSurveyFilesSelected)
        {
            await SurveyFilesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await SurveyFilesAppService.DeleteByIdsAsync(SelectedSurveyFiles.Select(x => x.SurveyFile.Id).ToList());
        }

        SelectedSurveyFiles.Clear();
        AllSurveyFilesSelected = false;
        await GetSurveyFilesAsync();
    }
}