using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using HC.NotificationReceivers;
using HC.Permissions;

namespace HC.Blazor.Pages;

public partial class NotificationsRead
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
    protected PageToolbar Toolbar { get; } = new PageToolbar();

    public DataGrid<NotificationReceiverWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<NotificationReceiverWithNavigationPropertiesDto> NotificationList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private GetNotificationReceiversInput Filter { get; set; }

    public NotificationsRead()
    {
        Filter = new GetNotificationReceiversInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
            IsRead = true
        };
        NotificationList = new List<NotificationReceiverWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ReadNotifications"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    private async Task GetNotificationsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await NotificationReceiversAppService.GetListAsync(Filter);
        NotificationList = result.Items;
        TotalCount = (int)result.TotalCount;
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetNotificationsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<NotificationReceiverWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetNotificationsAsync();
        await InvokeAsync(StateHasChanged);
    }
}

