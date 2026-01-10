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
using HC.CalendarEvents;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class CalendarEvents
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<CalendarEventDto> DataGridRef { get; set; }

    private IReadOnlyList<CalendarEventDto> CalendarEventList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateCalendarEvent { get; set; }

    private bool CanEditCalendarEvent { get; set; }

    private bool CanDeleteCalendarEvent { get; set; }

    private CalendarEventCreateDto NewCalendarEvent { get; set; }

    private Validations NewCalendarEventValidations { get; set; } = new();
    private CalendarEventUpdateDto EditingCalendarEvent { get; set; }

    private Validations EditingCalendarEventValidations { get; set; } = new();
    private Guid EditingCalendarEventId { get; set; }

    private Modal CreateCalendarEventModal { get; set; } = new();
    private Modal EditCalendarEventModal { get; set; } = new();
    private GetCalendarEventsInput Filter { get; set; }

    private DataGridEntityActionsColumn<CalendarEventDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "calendarEvent-create-tab";
    protected string SelectedEditTab = "calendarEvent-edit-tab";
    private CalendarEventDto? SelectedCalendarEvent;

    private List<CalendarEventDto> SelectedCalendarEvents { get; set; } = new();
    private bool AllCalendarEventsSelected { get; set; }

    public CalendarEvents()
    {
        NewCalendarEvent = new CalendarEventCreateDto();
        EditingCalendarEvent = new CalendarEventUpdateDto();
        Filter = new GetCalendarEventsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        CalendarEventList = new List<CalendarEventDto>();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["CalendarEvents"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewCalendarEvent"], async () => {
            await OpenCreateCalendarEventModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.CalendarEvents.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(CalendarEventDto calendarEvent)
    {
        DataGridRef.ToggleDetailRow(calendarEvent, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<CalendarEventDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteCalendarEvent;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<CalendarEventDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateCalendarEvent = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEvents.Create);
        CanEditCalendarEvent = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEvents.Edit);
        CanDeleteCalendarEvent = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEvents.Delete);
    }

    private async Task GetCalendarEventsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await CalendarEventsAppService.GetListAsync(Filter);
        CalendarEventList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetCalendarEventsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await CalendarEventsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/calendar-events/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Description={HttpUtility.UrlEncode(Filter.Description)}&StartTimeMin={Filter.StartTimeMin?.ToString("O")}&StartTimeMax={Filter.StartTimeMax?.ToString("O")}&EndTimeMin={Filter.EndTimeMin?.ToString("O")}&EndTimeMax={Filter.EndTimeMax?.ToString("O")}&AllDay={Filter.AllDay}&EventType={HttpUtility.UrlEncode(Filter.EventType?.ToString())}&Location={HttpUtility.UrlEncode(Filter.Location)}&RelatedType={HttpUtility.UrlEncode(Filter.RelatedType?.ToString())}&RelatedId={HttpUtility.UrlEncode(Filter.RelatedId)}&Visibility={HttpUtility.UrlEncode(Filter.Visibility?.ToString())}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CalendarEventDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetCalendarEventsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateCalendarEventModalAsync()
    {
        NewCalendarEvent = new CalendarEventCreateDto
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
        };
        SelectedCreateTab = "calendarEvent-create-tab";
        await NewCalendarEventValidations.ClearAll();
        await CreateCalendarEventModal.Show();
    }

    private async Task CloseCreateCalendarEventModalAsync()
    {
        NewCalendarEvent = new CalendarEventCreateDto
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
        };
        await CreateCalendarEventModal.Hide();
    }

    private async Task OpenEditCalendarEventModalAsync(CalendarEventDto input)
    {
        SelectedEditTab = "calendarEvent-edit-tab";
        var calendarEvent = await CalendarEventsAppService.GetAsync(input.Id);
        EditingCalendarEventId = calendarEvent.Id;
        EditingCalendarEvent = ObjectMapper.Map<CalendarEventDto, CalendarEventUpdateDto>(calendarEvent);
        await EditingCalendarEventValidations.ClearAll();
        await EditCalendarEventModal.Show();
    }

    private async Task DeleteCalendarEventAsync(CalendarEventDto input)
    {
        try
        {
            await CalendarEventsAppService.DeleteAsync(input.Id);
            await GetCalendarEventsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateCalendarEventAsync()
    {
        try
        {
            if (await NewCalendarEventValidations.ValidateAll() == false)
            {
                return;
            }

            await CalendarEventsAppService.CreateAsync(NewCalendarEvent);
            await GetCalendarEventsAsync();
            await CloseCreateCalendarEventModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditCalendarEventModalAsync()
    {
        await EditCalendarEventModal.Hide();
    }

    private async Task UpdateCalendarEventAsync()
    {
        try
        {
            if (await EditingCalendarEventValidations.ValidateAll() == false)
            {
                return;
            }

            await CalendarEventsAppService.UpdateAsync(EditingCalendarEventId, EditingCalendarEvent);
            await GetCalendarEventsAsync();
            await EditCalendarEventModal.Hide();
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

    protected virtual async Task OnDescriptionChangedAsync(string? description)
    {
        Filter.Description = description;
        await SearchAsync();
    }

    protected virtual async Task OnStartTimeMinChangedAsync(DateTime? startTimeMin)
    {
        Filter.StartTimeMin = startTimeMin.HasValue ? startTimeMin.Value.Date : startTimeMin;
        await SearchAsync();
    }

    protected virtual async Task OnStartTimeMaxChangedAsync(DateTime? startTimeMax)
    {
        Filter.StartTimeMax = startTimeMax.HasValue ? startTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : startTimeMax;
        await SearchAsync();
    }

    protected virtual async Task OnEndTimeMinChangedAsync(DateTime? endTimeMin)
    {
        Filter.EndTimeMin = endTimeMin.HasValue ? endTimeMin.Value.Date : endTimeMin;
        await SearchAsync();
    }

    protected virtual async Task OnEndTimeMaxChangedAsync(DateTime? endTimeMax)
    {
        Filter.EndTimeMax = endTimeMax.HasValue ? endTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : endTimeMax;
        await SearchAsync();
    }

    protected virtual async Task OnAllDayChangedAsync(bool? allDay)
    {
        Filter.AllDay = allDay;
        await SearchAsync();
    }

    protected virtual async Task OnEventTypeChangedAsync(string? eventType)
    {
        Filter.EventType = eventType;
        await SearchAsync();
    }

    protected virtual async Task OnLocationChangedAsync(string? location)
    {
        Filter.Location = location;
        await SearchAsync();
    }

    protected virtual async Task OnRelatedTypeChangedAsync(string? relatedType)
    {
        Filter.RelatedType = relatedType;
        await SearchAsync();
    }

    protected virtual async Task OnRelatedIdChangedAsync(string? relatedId)
    {
        Filter.RelatedId = relatedId;
        await SearchAsync();
    }

    protected virtual async Task OnVisibilityChangedAsync(string? visibility)
    {
        Filter.Visibility = visibility;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllCalendarEventsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllCalendarEventsSelected = false;
        SelectedCalendarEvents.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedCalendarEventRowsChanged()
    {
        if (SelectedCalendarEvents.Count != PageSize)
        {
            AllCalendarEventsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedCalendarEventsAsync()
    {
        var message = AllCalendarEventsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedCalendarEvents.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllCalendarEventsSelected)
        {
            await CalendarEventsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await CalendarEventsAppService.DeleteByIdsAsync(SelectedCalendarEvents.Select(x => x.Id).ToList());
        }

        SelectedCalendarEvents.Clear();
        AllCalendarEventsSelected = false;
        await GetCalendarEventsAsync();
    }
}