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
using HC.Notifications;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class Notifications
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<NotificationDto> DataGridRef { get; set; }

    private IReadOnlyList<NotificationDto> NotificationList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateNotification { get; set; }

    private bool CanEditNotification { get; set; }

    private bool CanDeleteNotification { get; set; }

    private NotificationCreateDto NewNotification { get; set; }

    private Validations NewNotificationValidations { get; set; } = new();
    private NotificationUpdateDto EditingNotification { get; set; }

    private Validations EditingNotificationValidations { get; set; } = new();
    private Guid EditingNotificationId { get; set; }

    private Modal CreateNotificationModal { get; set; } = new();
    private Modal EditNotificationModal { get; set; } = new();
    private GetNotificationsInput Filter { get; set; }

    private DataGridEntityActionsColumn<NotificationDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "notification-create-tab";
    protected string SelectedEditTab = "notification-edit-tab";
    private NotificationDto? SelectedNotification;

    private List<NotificationDto> SelectedNotifications { get; set; } = new();
    private bool AllNotificationsSelected { get; set; }

    public Notifications()
    {
        NewNotification = new NotificationCreateDto();
        EditingNotification = new NotificationUpdateDto();
        Filter = new GetNotificationsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        NotificationList = new List<NotificationDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Notifications"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewNotification"], async () => {
            await OpenCreateNotificationModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.Notifications.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(NotificationDto notification)
    {
        DataGridRef.ToggleDetailRow(notification, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<NotificationDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteNotification;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<NotificationDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateNotification = await AuthorizationService.IsGrantedAsync(HCPermissions.Notifications.Create);
        CanEditNotification = await AuthorizationService.IsGrantedAsync(HCPermissions.Notifications.Edit);
        CanDeleteNotification = await AuthorizationService.IsGrantedAsync(HCPermissions.Notifications.Delete);
    }

    private async Task GetNotificationsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await NotificationsAppService.GetListAsync(Filter);
        NotificationList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetNotificationsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await NotificationsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/notifications/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Content={HttpUtility.UrlEncode(Filter.Content)}&SourceType={HttpUtility.UrlEncode(Filter.SourceType.ToString())}&EventType={HttpUtility.UrlEncode(Filter.EventType.ToString())}&RelatedType={HttpUtility.UrlEncode(Filter.RelatedType.ToString())}&RelatedId={HttpUtility.UrlEncode(Filter.RelatedId)}&Priority={HttpUtility.UrlEncode(Filter.Priority)}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<NotificationDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetNotificationsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateNotificationModalAsync()
    {
        NewNotification = new NotificationCreateDto
        {
            SourceType = SourceType.WORKFLOW,
            EventType = EventType.WORKFLOW_ASSIGNED,
            RelatedType = RelatedType.DOCUMENT,
        };
        SelectedCreateTab = "notification-create-tab";
        await NewNotificationValidations.ClearAll();
        await CreateNotificationModal.Show();
    }

    private async Task CloseCreateNotificationModalAsync()
    {
        NewNotification = new NotificationCreateDto
        {
        };
        await CreateNotificationModal.Hide();
    }

    private async Task OpenEditNotificationModalAsync(NotificationDto input)
    {
        SelectedEditTab = "notification-edit-tab";
        var notification = await NotificationsAppService.GetAsync(input.Id);
        EditingNotificationId = notification.Id;
        EditingNotification = ObjectMapper.Map<NotificationDto, NotificationUpdateDto>(notification);
        await EditingNotificationValidations.ClearAll();
        await EditNotificationModal.Show();
    }

    private async Task DeleteNotificationAsync(NotificationDto input)
    {
        try
        {
            await NotificationsAppService.DeleteAsync(input.Id);
            await GetNotificationsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateNotificationAsync()
    {
        try
        {
            if (await NewNotificationValidations.ValidateAll() == false)
            {
                return;
            }

            await NotificationsAppService.CreateAsync(NewNotification);
            await GetNotificationsAsync();
            await CloseCreateNotificationModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditNotificationModalAsync()
    {
        await EditNotificationModal.Hide();
    }

    private async Task UpdateNotificationAsync()
    {
        try
        {
            if (await EditingNotificationValidations.ValidateAll() == false)
            {
                return;
            }

            await NotificationsAppService.UpdateAsync(EditingNotificationId, EditingNotification);
            await GetNotificationsAsync();
            await EditNotificationModal.Hide();
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

    protected virtual async Task OnTitleChangedAsync(string? title)
    {
        Filter.Title = title;
        await SearchAsync();
    }

    protected virtual async Task OnContentChangedAsync(string? content)
    {
        Filter.Content = content;
        await SearchAsync();
    }

    protected virtual async Task OnSourceTypeChangedAsync(string? sourceType)
    {
        if (string.IsNullOrWhiteSpace(sourceType))
        {
            Filter.SourceType = null;
        }
        else
        {
            Filter.SourceType = Enum.Parse<SourceType>(sourceType);
        }
        await SearchAsync();
    }

    protected virtual async Task OnEventTypeChangedAsync(string? eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            Filter.EventType = null;
        }
        else
        {
            Filter.EventType = Enum.Parse<EventType>(eventType);
        }
        await SearchAsync();
    }

    protected virtual async Task OnRelatedTypeChangedAsync(string? relatedType)
    {
        if (string.IsNullOrWhiteSpace(relatedType))
        {
            Filter.RelatedType = null;
        }
        else
        {
            Filter.RelatedType = Enum.Parse<RelatedType>(relatedType);
        }

        await SearchAsync();
    }

    protected virtual async Task OnRelatedIdChangedAsync(string? relatedId)
    {
        Filter.RelatedId = relatedId;
        await SearchAsync();
    }

    protected virtual async Task OnPriorityChangedAsync(string? priority)
    {
        Filter.Priority = priority;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllNotificationsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllNotificationsSelected = false;
        SelectedNotifications.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedNotificationRowsChanged()
    {
        if (SelectedNotifications.Count != PageSize)
        {
            AllNotificationsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedNotificationsAsync()
    {
        var message = AllNotificationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedNotifications.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllNotificationsSelected)
        {
            await NotificationsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await NotificationsAppService.DeleteByIdsAsync(SelectedNotifications.Select(x => x.Id).ToList());
        }

        SelectedNotifications.Clear();
        AllNotificationsSelected = false;
        await GetNotificationsAsync();
    }
}