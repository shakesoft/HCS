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
using HC.NotificationReceivers;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class NotificationReceivers
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<NotificationReceiverWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<NotificationReceiverWithNavigationPropertiesDto> NotificationReceiverList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateNotificationReceiver { get; set; }

    private bool CanEditNotificationReceiver { get; set; }

    private bool CanDeleteNotificationReceiver { get; set; }

    private NotificationReceiverCreateDto NewNotificationReceiver { get; set; }

    private Validations NewNotificationReceiverValidations { get; set; } = new();
    private NotificationReceiverUpdateDto EditingNotificationReceiver { get; set; }

    private Validations EditingNotificationReceiverValidations { get; set; } = new();
    private Guid EditingNotificationReceiverId { get; set; }

    private Modal CreateNotificationReceiverModal { get; set; } = new();
    private Modal EditNotificationReceiverModal { get; set; } = new();
    private GetNotificationReceiversInput Filter { get; set; }

    private DataGridEntityActionsColumn<NotificationReceiverWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "notificationReceiver-create-tab";
    protected string SelectedEditTab = "notificationReceiver-edit-tab";
    private NotificationReceiverWithNavigationPropertiesDto? SelectedNotificationReceiver;

    private IReadOnlyList<LookupDto<Guid>> NotificationsCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<NotificationReceiverWithNavigationPropertiesDto> SelectedNotificationReceivers { get; set; } = new();
    private bool AllNotificationReceiversSelected { get; set; }

    public NotificationReceivers()
    {
        NewNotificationReceiver = new NotificationReceiverCreateDto();
        EditingNotificationReceiver = new NotificationReceiverUpdateDto();
        Filter = new GetNotificationReceiversInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        NotificationReceiverList = new List<NotificationReceiverWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetNotificationCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["NotificationReceivers"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewNotificationReceiver"], async () => {
            await OpenCreateNotificationReceiverModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.NotificationReceivers.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(NotificationReceiverWithNavigationPropertiesDto notificationReceiver)
    {
        DataGridRef.ToggleDetailRow(notificationReceiver, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<NotificationReceiverWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteNotificationReceiver;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<NotificationReceiverWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateNotificationReceiver = await AuthorizationService.IsGrantedAsync(HCPermissions.NotificationReceivers.Create);
        CanEditNotificationReceiver = await AuthorizationService.IsGrantedAsync(HCPermissions.NotificationReceivers.Edit);
        CanDeleteNotificationReceiver = await AuthorizationService.IsGrantedAsync(HCPermissions.NotificationReceivers.Delete);
    }

    private async Task GetNotificationReceiversAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await NotificationReceiversAppService.GetListAsync(Filter);
        NotificationReceiverList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetNotificationReceiversAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await NotificationReceiversAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/notification-receivers/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&IsRead={Filter.IsRead}&ReadAtMin={Filter.ReadAtMin?.ToString("O")}&ReadAtMax={Filter.ReadAtMax?.ToString("O")}&NotificationId={Filter.NotificationId}&IdentityUserId={Filter.IdentityUserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<NotificationReceiverWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetNotificationReceiversAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateNotificationReceiverModalAsync()
    {
        NewNotificationReceiver = new NotificationReceiverCreateDto
        {
            ReadAt = DateTime.Now,
            NotificationId = NotificationsCollection.Select(i => i.Id).FirstOrDefault(),
            IdentityUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "notificationReceiver-create-tab";
        await NewNotificationReceiverValidations.ClearAll();
        await CreateNotificationReceiverModal.Show();
    }

    private async Task CloseCreateNotificationReceiverModalAsync()
    {
        NewNotificationReceiver = new NotificationReceiverCreateDto
        {
            ReadAt = DateTime.Now,
            NotificationId = NotificationsCollection.Select(i => i.Id).FirstOrDefault(),
            IdentityUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateNotificationReceiverModal.Hide();
    }

    private async Task OpenEditNotificationReceiverModalAsync(NotificationReceiverWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "notificationReceiver-edit-tab";
        var notificationReceiver = await NotificationReceiversAppService.GetWithNavigationPropertiesAsync(input.NotificationReceiver.Id);
        EditingNotificationReceiverId = notificationReceiver.NotificationReceiver.Id;
        EditingNotificationReceiver = ObjectMapper.Map<NotificationReceiverDto, NotificationReceiverUpdateDto>(notificationReceiver.NotificationReceiver);
        await EditingNotificationReceiverValidations.ClearAll();
        await EditNotificationReceiverModal.Show();
    }

    private async Task DeleteNotificationReceiverAsync(NotificationReceiverWithNavigationPropertiesDto input)
    {
        try
        {
            await NotificationReceiversAppService.DeleteAsync(input.NotificationReceiver.Id);
            await GetNotificationReceiversAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteNotificationReceiverWithConfirmationAsync(NotificationReceiverWithNavigationPropertiesDto input)
    {
        if (await UiMessageService.Confirm(L["DeleteConfirmationMessage"].Value))
        {
            await DeleteNotificationReceiverAsync(input);
        }
    }

    private async Task CreateNotificationReceiverAsync()
    {
        try
        {
            if (await NewNotificationReceiverValidations.ValidateAll() == false)
            {
                return;
            }

            await NotificationReceiversAppService.CreateAsync(NewNotificationReceiver);
            await GetNotificationReceiversAsync();
            await CloseCreateNotificationReceiverModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditNotificationReceiverModalAsync()
    {
        await EditNotificationReceiverModal.Hide();
    }

    private async Task UpdateNotificationReceiverAsync()
    {
        try
        {
            if (await EditingNotificationReceiverValidations.ValidateAll() == false)
            {
                return;
            }

            await NotificationReceiversAppService.UpdateAsync(EditingNotificationReceiverId, EditingNotificationReceiver);
            await GetNotificationReceiversAsync();
            await EditNotificationReceiverModal.Hide();
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

    protected virtual async Task OnIsReadChangedAsync(bool? isRead)
    {
        Filter.IsRead = isRead;
        await SearchAsync();
    }

    protected virtual async Task OnReadAtMinChangedAsync(DateTime? readAtMin)
    {
        Filter.ReadAtMin = readAtMin.HasValue ? readAtMin.Value.Date : readAtMin;
        await SearchAsync();
    }

    protected virtual async Task OnReadAtMaxChangedAsync(DateTime? readAtMax)
    {
        Filter.ReadAtMax = readAtMax.HasValue ? readAtMax.Value.Date.AddDays(1).AddSeconds(-1) : readAtMax;
        await SearchAsync();
    }

    protected virtual async Task OnNotificationIdChangedAsync(Guid? notificationId)
    {
        Filter.NotificationId = notificationId;
        await SearchAsync();
    }

    protected virtual async Task OnIdentityUserIdChangedAsync(Guid? identityUserId)
    {
        Filter.IdentityUserId = identityUserId;
        await SearchAsync();
    }

    private async Task GetNotificationCollectionLookupAsync(string? newValue = null)
    {
        NotificationsCollection = (await NotificationReceiversAppService.GetNotificationLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await NotificationReceiversAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllNotificationReceiversSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllNotificationReceiversSelected = false;
        SelectedNotificationReceivers.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedNotificationReceiverRowsChanged()
    {
        if (SelectedNotificationReceivers.Count != PageSize)
        {
            AllNotificationReceiversSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedNotificationReceiversAsync()
    {
        var message = AllNotificationReceiversSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedNotificationReceivers.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllNotificationReceiversSelected)
        {
            await NotificationReceiversAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await NotificationReceiversAppService.DeleteByIdsAsync(SelectedNotificationReceivers.Select(x => x.NotificationReceiver.Id).ToList());
        }

        SelectedNotificationReceivers.Clear();
        AllNotificationReceiversSelected = false;
        await GetNotificationReceiversAsync();
    }
}