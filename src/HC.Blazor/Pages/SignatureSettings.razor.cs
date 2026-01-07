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
using HC.SignatureSettings;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class SignatureSettings
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<SignatureSettingDto> DataGridRef { get; set; }

    private IReadOnlyList<SignatureSettingDto> SignatureSettingList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateSignatureSetting { get; set; }

    private bool CanEditSignatureSetting { get; set; }

    private bool CanDeleteSignatureSetting { get; set; }

    private SignatureSettingCreateDto NewSignatureSetting { get; set; }

    private Validations NewSignatureSettingValidations { get; set; } = new();
    private SignatureSettingUpdateDto EditingSignatureSetting { get; set; }

    private Validations EditingSignatureSettingValidations { get; set; } = new();
    private Guid EditingSignatureSettingId { get; set; }

    private Modal CreateSignatureSettingModal { get; set; } = new();
    private Modal EditSignatureSettingModal { get; set; } = new();
    private GetSignatureSettingsInput Filter { get; set; }

    private DataGridEntityActionsColumn<SignatureSettingDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "signatureSetting-create-tab";
    protected string SelectedEditTab = "signatureSetting-edit-tab";
    private SignatureSettingDto? SelectedSignatureSetting;

    private List<SignatureSettingDto> SelectedSignatureSettings { get; set; } = new();
    private bool AllSignatureSettingsSelected { get; set; }

    public SignatureSettings()
    {
        NewSignatureSetting = new SignatureSettingCreateDto();
        EditingSignatureSetting = new SignatureSettingUpdateDto();
        Filter = new GetSignatureSettingsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        SignatureSettingList = new List<SignatureSettingDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SignatureSettings"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewSignatureSetting"], async () => {
            await OpenCreateSignatureSettingModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.SignatureSettings.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(SignatureSettingDto signatureSetting)
    {
        DataGridRef.ToggleDetailRow(signatureSetting, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<SignatureSettingDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteSignatureSetting;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<SignatureSettingDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateSignatureSetting = await AuthorizationService.IsGrantedAsync(HCPermissions.SignatureSettings.Create);
        CanEditSignatureSetting = await AuthorizationService.IsGrantedAsync(HCPermissions.SignatureSettings.Edit);
        CanDeleteSignatureSetting = await AuthorizationService.IsGrantedAsync(HCPermissions.SignatureSettings.Delete);
    }

    private async Task GetSignatureSettingsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await SignatureSettingsAppService.GetListAsync(Filter);
        SignatureSettingList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetSignatureSettingsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await SignatureSettingsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/signature-settings/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ProviderCode={HttpUtility.UrlEncode(Filter.ProviderCode)}&ProviderType={HttpUtility.UrlEncode(Filter.ProviderType)}&ApiEndpoint={HttpUtility.UrlEncode(Filter.ApiEndpoint)}&ApiTimeoutMin={Filter.ApiTimeoutMin}&ApiTimeoutMax={Filter.ApiTimeoutMax}&DefaultSignType={HttpUtility.UrlEncode(Filter.DefaultSignType)}&AllowElectronicSign={Filter.AllowElectronicSign}&AllowDigitalSign={Filter.AllowDigitalSign}&RequireOtp={Filter.RequireOtp}&SignWidthMin={Filter.SignWidthMin}&SignWidthMax={Filter.SignWidthMax}&SignHeightMin={Filter.SignHeightMin}&SignHeightMax={Filter.SignHeightMax}&SignedFileSuffix={HttpUtility.UrlEncode(Filter.SignedFileSuffix)}&KeepOriginalFile={Filter.KeepOriginalFile}&OverwriteSignedFile={Filter.OverwriteSignedFile}&EnableSignLog={Filter.EnableSignLog}&IsActive={Filter.IsActive}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SignatureSettingDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetSignatureSettingsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateSignatureSettingModalAsync()
    {
        NewSignatureSetting = new SignatureSettingCreateDto
        {
        };
        SelectedCreateTab = "signatureSetting-create-tab";
        await NewSignatureSettingValidations.ClearAll();
        await CreateSignatureSettingModal.Show();
    }

    private async Task CloseCreateSignatureSettingModalAsync()
    {
        NewSignatureSetting = new SignatureSettingCreateDto
        {
        };
        await CreateSignatureSettingModal.Hide();
    }

    private async Task OpenEditSignatureSettingModalAsync(SignatureSettingDto input)
    {
        SelectedEditTab = "signatureSetting-edit-tab";
        var signatureSetting = await SignatureSettingsAppService.GetAsync(input.Id);
        EditingSignatureSettingId = signatureSetting.Id;
        EditingSignatureSetting = ObjectMapper.Map<SignatureSettingDto, SignatureSettingUpdateDto>(signatureSetting);
        await EditingSignatureSettingValidations.ClearAll();
        await EditSignatureSettingModal.Show();
    }

    private async Task DeleteSignatureSettingAsync(SignatureSettingDto input)
    {
        try
        {
            await SignatureSettingsAppService.DeleteAsync(input.Id);
            await GetSignatureSettingsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateSignatureSettingAsync()
    {
        try
        {
            if (await NewSignatureSettingValidations.ValidateAll() == false)
            {
                return;
            }

            await SignatureSettingsAppService.CreateAsync(NewSignatureSetting);
            await GetSignatureSettingsAsync();
            await CloseCreateSignatureSettingModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditSignatureSettingModalAsync()
    {
        await EditSignatureSettingModal.Hide();
    }

    private async Task UpdateSignatureSettingAsync()
    {
        try
        {
            if (await EditingSignatureSettingValidations.ValidateAll() == false)
            {
                return;
            }

            await SignatureSettingsAppService.UpdateAsync(EditingSignatureSettingId, EditingSignatureSetting);
            await GetSignatureSettingsAsync();
            await EditSignatureSettingModal.Hide();
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

    protected virtual async Task OnProviderCodeChangedAsync(string? providerCode)
    {
        Filter.ProviderCode = providerCode;
        await SearchAsync();
    }

    protected virtual async Task OnProviderTypeChangedAsync(string? providerType)
    {
        Filter.ProviderType = providerType;
        await SearchAsync();
    }

    protected virtual async Task OnApiEndpointChangedAsync(string? apiEndpoint)
    {
        Filter.ApiEndpoint = apiEndpoint;
        await SearchAsync();
    }

    protected virtual async Task OnApiTimeoutMinChangedAsync(int? apiTimeoutMin)
    {
        Filter.ApiTimeoutMin = apiTimeoutMin;
        await SearchAsync();
    }

    protected virtual async Task OnApiTimeoutMaxChangedAsync(int? apiTimeoutMax)
    {
        Filter.ApiTimeoutMax = apiTimeoutMax;
        await SearchAsync();
    }

    protected virtual async Task OnDefaultSignTypeChangedAsync(string? defaultSignType)
    {
        Filter.DefaultSignType = defaultSignType;
        await SearchAsync();
    }

    protected virtual async Task OnAllowElectronicSignChangedAsync(bool? allowElectronicSign)
    {
        Filter.AllowElectronicSign = allowElectronicSign;
        await SearchAsync();
    }

    protected virtual async Task OnAllowDigitalSignChangedAsync(bool? allowDigitalSign)
    {
        Filter.AllowDigitalSign = allowDigitalSign;
        await SearchAsync();
    }

    protected virtual async Task OnRequireOtpChangedAsync(bool? requireOtp)
    {
        Filter.RequireOtp = requireOtp;
        await SearchAsync();
    }

    protected virtual async Task OnSignWidthMinChangedAsync(int? signWidthMin)
    {
        Filter.SignWidthMin = signWidthMin;
        await SearchAsync();
    }

    protected virtual async Task OnSignWidthMaxChangedAsync(int? signWidthMax)
    {
        Filter.SignWidthMax = signWidthMax;
        await SearchAsync();
    }

    protected virtual async Task OnSignHeightMinChangedAsync(int? signHeightMin)
    {
        Filter.SignHeightMin = signHeightMin;
        await SearchAsync();
    }

    protected virtual async Task OnSignHeightMaxChangedAsync(int? signHeightMax)
    {
        Filter.SignHeightMax = signHeightMax;
        await SearchAsync();
    }

    protected virtual async Task OnSignedFileSuffixChangedAsync(string? signedFileSuffix)
    {
        Filter.SignedFileSuffix = signedFileSuffix;
        await SearchAsync();
    }

    protected virtual async Task OnKeepOriginalFileChangedAsync(bool? keepOriginalFile)
    {
        Filter.KeepOriginalFile = keepOriginalFile;
        await SearchAsync();
    }

    protected virtual async Task OnOverwriteSignedFileChangedAsync(bool? overwriteSignedFile)
    {
        Filter.OverwriteSignedFile = overwriteSignedFile;
        await SearchAsync();
    }

    protected virtual async Task OnEnableSignLogChangedAsync(bool? enableSignLog)
    {
        Filter.EnableSignLog = enableSignLog;
        await SearchAsync();
    }

    protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
    {
        Filter.IsActive = isActive;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllSignatureSettingsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllSignatureSettingsSelected = false;
        SelectedSignatureSettings.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedSignatureSettingRowsChanged()
    {
        if (SelectedSignatureSettings.Count != PageSize)
        {
            AllSignatureSettingsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedSignatureSettingsAsync()
    {
        var message = AllSignatureSettingsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSignatureSettings.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllSignatureSettingsSelected)
        {
            await SignatureSettingsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await SignatureSettingsAppService.DeleteByIdsAsync(SelectedSignatureSettings.Select(x => x.Id).ToList());
        }

        SelectedSignatureSettings.Clear();
        AllSignatureSettingsSelected = false;
        await GetSignatureSettingsAsync();
    }
}