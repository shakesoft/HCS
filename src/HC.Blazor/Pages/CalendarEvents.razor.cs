using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using System.Threading;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Scheduler;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using HC.CalendarEvents;
using HC.ProjectTasks;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace HC.Blazor.Pages;


public partial class CalendarEvents : HCComponentBase
{
    [Inject] private IProjectTasksAppService ProjectTasksAppService { get; set; } = default!;
    [Inject] private IMemoryCache __MemoryCache { get; set; } = default!;
    [Inject] private ILogger<CalendarEvents> Logger { get; set; } = default!;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; set; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    // View toggle
    protected bool IsListView { get; set; } = true;

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

    private CalendarEventUpdateDto EditingCalendarEvent { get; set; }
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

    // Enum properties for Create
    private EventType NewCalendarEventEventType { get; set; } = EventType.MEETING;
    private RelatedType NewCalendarEventRelatedType { get; set; } = RelatedType.NONE;
    private EventVisibility NewCalendarEventVisibility { get; set; } = EventVisibility.PRIVATE;

    // Enum properties for Edit
    private EventType EditingCalendarEventEventType { get; set; } = EventType.MEETING;
    private RelatedType EditingCalendarEventRelatedType { get; set; } = RelatedType.NONE;
    private EventVisibility EditingCalendarEventVisibility { get; set; } = EventVisibility.PRIVATE;

    // Select2 for Projects
    private IReadOnlyList<LookupDto<Guid>> ProjectsCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<LookupDto<Guid>> SelectedNewProject { get; set; } = new();
    private List<LookupDto<Guid>> SelectedEditProject { get; set; } = new();

    // Select2 for ProjectTasks
    protected sealed class ProjectTaskSelectItem
    {
        public string Id { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
    }
    private IReadOnlyList<ProjectTaskSelectItem> ProjectTasksCollection { get; set; } = new List<ProjectTaskSelectItem>();
    private List<ProjectTaskSelectItem> SelectedNewProjectTask { get; set; } = new();
    private List<ProjectTaskSelectItem> SelectedEditProjectTask { get; set; } = new();

    // Field-level validation errors
    private Dictionary<string, string?> CreateFieldErrors { get; set; } = new();
    private Dictionary<string, string?> EditFieldErrors { get; set; } = new();

    // Validation error keys
    private string? CreateCalendarEventValidationErrorKey { get; set; }
    private string? EditCalendarEventValidationErrorKey { get; set; }

    // Helper methods to get field errors
    private string? GetCreateFieldError(string fieldName) => CreateFieldErrors.GetValueOrDefault(fieldName);
    private string? GetEditFieldError(string fieldName) => EditFieldErrors.GetValueOrDefault(fieldName);
    private bool HasCreateFieldError(string fieldName) => CreateFieldErrors.ContainsKey(fieldName) && !string.IsNullOrWhiteSpace(CreateFieldErrors[fieldName]);
    private bool HasEditFieldError(string fieldName) => EditFieldErrors.ContainsKey(fieldName) && !string.IsNullOrWhiteSpace(EditFieldErrors[fieldName]);

    // Scheduler properties
    private DateOnly SelectedSchedulerDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    private SchedulerView SelectedSchedulerView { get; set; } = SchedulerView.Week;
    private TimeOnly SchedulerStartTime { get; set; } = new TimeOnly(00, 00);
    private TimeOnly SchedulerEndTime { get; set; } = new TimeOnly(23, 59);
    private TimeOnly SchedulerWorkDayStart { get; set; } = new TimeOnly(00, 00);
    private TimeOnly SchedulerWorkDayEnd { get; set; } = new TimeOnly(23, 59);

    // Scheduler Localizers
    private SchedulerLocalizers SchedulerLocalizers { get; set; } = new();

    // DatePicker refs for StartTime and EndTime
    private DatePicker<DateTime>? NewCalendarEventStartTimeDatePicker { get; set; }
    private DatePicker<DateTime>? NewCalendarEventEndTimeDatePicker { get; set; }
    private DatePicker<DateTime>? EditingCalendarEventStartTimeDatePicker { get; set; }
    private DatePicker<DateTime>? EditingCalendarEventEndTimeDatePicker { get; set; }

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
        Logger.LogInformation("OnInitializedAsync - Starting initialization, IsListView: {IsListView}", IsListView);
        
        await SetPermissionsAsync();
        InitializeSchedulerLocalizers();
        try
        {
            await GetProjectCollectionLookupAsync();
            // Load calendar events on initialization
            await GetCalendarEventsAsync();
            
            Logger.LogInformation("OnInitializedAsync - Initialization complete, CalendarEventList count: {Count}", CalendarEventList.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnInitializedAsync - Error during initialization: {Message}", ex.Message);
            // Log error but don't block initialization
            await HandleErrorAsync(ex);
        }
    }

    private void InitializeSchedulerLocalizers()
    {
        SchedulerLocalizers = new SchedulerLocalizers
        {
            TodayLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Today"]),
            DayLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Day"]),
            WeekLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Week"]),
            WorkWeekLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["WorkWeek"]),
            MonthLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Month"]),
            YearLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Year"]),
            OnLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["On"]),
            WeekOfMonthLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["WeekOfMonth"]),
            DayOfWeekLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["DayOfWeek"]),
            MonthOfYearLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["MonthOfYear"]),
            CountLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Count"]),
            NeverLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["Never"]),
            RepeatEveryLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["RepeatEvery"]),
            WhatDoYouWantToDoLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["WhatDoYouWantToDo"]),
            RecurringSeriesWhatDoYouWantToDoLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["RecurringSeriesWhatDoYouWantToDo"]),
            StartDateHigherValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["StartDateHigherValidation"]),
            StartDateHigherOrEqualValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["StartDateHigherOrEqualValidation"]),
            EndDateLowerValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["EndDateLowerValidation"]),
            EndDateLowerOrEqualValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["EndDateLowerOrEqualValidation"]),
            EndTimeLowerValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["EndTimeLowerValidation"]),
            TitleRequiredValidationLocalizer = new Blazorise.Localization.TextLocalizerHandler((name, arguments) => L["TitleRequiredValidation"]),
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Logger.LogInformation("OnAfterRenderAsync - First render, IsListView: {IsListView}, CalendarEventList count: {Count}",
                IsListView, CalendarEventList.Count);
            
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            // Ensure data is loaded for both views
            if (!CalendarEventList.Any())
            {
                Logger.LogInformation("OnAfterRenderAsync - CalendarEventList is empty, reloading data");
                await GetCalendarEventsAsync();
            }
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
        RebuildToolbar();
        return ValueTask.CompletedTask;
    }

    private void RebuildToolbar()
    {
        Toolbar = new PageToolbar();
        // Toggle button: show "Calendar" icon and text when in List view, "List" icon and text when in Calendar view
        Toolbar.AddButton(
            IsListView ? L["Calendar"] : L["List"],
            async () => { await ToggleViewAsync(); },
            IsListView ? IconName.Calendar : IconName.List);
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewCalendarEvent"], async () => {
            await OpenCreateCalendarEventModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.CalendarEvents.Create);
    }

    private async Task ToggleViewAsync()
    {
        Logger.LogInformation("ToggleViewAsync - Switching from {OldView} to {NewView}", 
            IsListView ? "List" : "Calendar", !IsListView ? "List" : "Calendar");
        
        IsListView = !IsListView;
        RebuildToolbar();
        
        // Always reload data when switching views to ensure correct pagination/all data
        await GetCalendarEventsAsync();
        
        Logger.LogInformation("ToggleViewAsync - After reload, CalendarEventList count: {Count}", CalendarEventList.Count);
        
        await InvokeAsync(StateHasChanged);
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
        try
        {
            // Create a new filter for Calendar view to load all events without pagination
            GetCalendarEventsInput calendarFilter;
            if (!IsListView)
            {
                // For Calendar view, load all events (or a large number) without pagination
                calendarFilter = new GetCalendarEventsInput
                {
                    MaxResultCount = 10000, // Load a large number of events for calendar view
                    SkipCount = 0,
                    Sorting = CurrentSorting
                };
                // Copy filter properties from existing Filter if available
                if (Filter != null)
                {
                    calendarFilter.FilterText = Filter.FilterText;
                    calendarFilter.Title = Filter.Title;
                    calendarFilter.Description = Filter.Description;
                    calendarFilter.StartTimeMin = Filter.StartTimeMin;
                    calendarFilter.StartTimeMax = Filter.StartTimeMax;
                    calendarFilter.EndTimeMin = Filter.EndTimeMin;
                    calendarFilter.EndTimeMax = Filter.EndTimeMax;
                    calendarFilter.AllDay = Filter.AllDay;
                    calendarFilter.EventType = Filter.EventType;
                    calendarFilter.Location = Filter.Location;
                    calendarFilter.RelatedType = Filter.RelatedType;
                    calendarFilter.RelatedId = Filter.RelatedId;
                    calendarFilter.Visibility = Filter.Visibility;
                }
            }
            else
            {
                // For List view, use pagination
                if (Filter == null)
                {
                    calendarFilter = new GetCalendarEventsInput
                    {
                        MaxResultCount = PageSize,
                        SkipCount = (CurrentPage - 1) * PageSize,
                        Sorting = CurrentSorting
                    };
                }
                else
                {
                    calendarFilter = Filter;
                    calendarFilter.MaxResultCount = PageSize;
                    calendarFilter.SkipCount = (CurrentPage - 1) * PageSize;
                    calendarFilter.Sorting = CurrentSorting;
                }
            }
            
            var result = await CalendarEventsAppService.GetListAsync(calendarFilter);
            CalendarEventList = result.Items ?? new List<CalendarEventDto>();
            TotalCount = (int)(result?.TotalCount ?? 0);
            
            // Log for debugging
            Logger.LogInformation("GetCalendarEventsAsync - IsListView: {IsListView}, TotalCount: {TotalCount}, ItemsCount: {ItemsCount}", 
                IsListView, TotalCount, CalendarEventList.Count);
            if (CalendarEventList.Any())
            {
                Logger.LogInformation("First event - Id: {Id}, Title: {Title}, StartTime: {StartTime}, EndTime: {EndTime}, AllDay: {AllDay}",
                    CalendarEventList.First().Id, CalendarEventList.First().Title, 
                    CalendarEventList.First().StartTime, CalendarEventList.First().EndTime, 
                    CalendarEventList.First().AllDay);
            }
            
            await ClearSelection();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            CalendarEventList = new List<CalendarEventDto>();
            TotalCount = 0;
        }
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
        try
        {
            CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
            CurrentPage = e.Page;
            await GetCalendarEventsAsync();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OpenCreateCalendarEventModalAsync()
    {
        NewCalendarEvent = new CalendarEventCreateDto
        {
            StartTime = DateTime.Now,
            EndTime = DateTime.Now,
        };
        NewCalendarEventEventType = EventType.MEETING;
        NewCalendarEventRelatedType = RelatedType.NONE;
        NewCalendarEventVisibility = EventVisibility.PRIVATE;
        SelectedNewProject = new List<LookupDto<Guid>>();
        SelectedNewProjectTask = new List<ProjectTaskSelectItem>();
        CreateFieldErrors.Clear();
        CreateCalendarEventValidationErrorKey = null;
        SelectedCreateTab = "calendarEvent-create-tab";
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
        
        // Parse enum values from string
        if (Enum.TryParse<EventType>(calendarEvent.EventType, out var eventType))
        {
            EditingCalendarEventEventType = eventType;
        }
        if (Enum.TryParse<RelatedType>(calendarEvent.RelatedType, out var relatedType))
        {
            EditingCalendarEventRelatedType = relatedType;
        }
        if (Enum.TryParse<EventVisibility>(calendarEvent.Visibility, out var visibility))
        {
            EditingCalendarEventVisibility = visibility;
        }

        // Set Select2 values
        SelectedEditProject = new List<LookupDto<Guid>>();
        SelectedEditProjectTask = new List<ProjectTaskSelectItem>();
        if (!string.IsNullOrWhiteSpace(calendarEvent.RelatedId))
        {
            if (EditingCalendarEventRelatedType == RelatedType.PROJECT && Guid.TryParse(calendarEvent.RelatedId, out var projectId))
            {
                await GetProjectCollectionLookupAsync();
                var project = ProjectsCollection.FirstOrDefault(p => p.Id == projectId);
                if (project != null)
                {
                    SelectedEditProject = new List<LookupDto<Guid>> { project };
                }
            }
            else if (EditingCalendarEventRelatedType == RelatedType.TASK)
            {
                await GetProjectTaskCollectionLookupAsync();
                var task = ProjectTasksCollection.FirstOrDefault(t => t.Id == calendarEvent.RelatedId);
                if (task != null)
                {
                    SelectedEditProjectTask = new List<ProjectTaskSelectItem> { task };
                }
            }
        }

        EditFieldErrors.Clear();
        EditCalendarEventValidationErrorKey = null;
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
            if (!ValidateCreateCalendarEvent())
            {
                await UiMessageService.Warn(L[CreateCalendarEventValidationErrorKey ?? "ValidationError"]);
                await InvokeAsync(StateHasChanged);
                return;
            }

            // Set enum values as strings
            NewCalendarEvent.EventType = NewCalendarEventEventType.ToString();
            NewCalendarEvent.RelatedType = NewCalendarEventRelatedType.ToString();
            NewCalendarEvent.Visibility = NewCalendarEventVisibility.ToString();

            // Set RelatedId based on RelatedType
            if (NewCalendarEventRelatedType == RelatedType.PROJECT)
            {
                NewCalendarEvent.RelatedId = SelectedNewProject.FirstOrDefault()?.Id.ToString();
            }
            else if (NewCalendarEventRelatedType == RelatedType.TASK)
            {
                NewCalendarEvent.RelatedId = SelectedNewProjectTask.FirstOrDefault()?.Id;
            }
            else
            {
                NewCalendarEvent.RelatedId = null;
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
            if (!ValidateEditCalendarEvent())
            {
                await UiMessageService.Warn(L[EditCalendarEventValidationErrorKey ?? "ValidationError"]);
                await InvokeAsync(StateHasChanged);
                return;
            }

            // Set enum values as strings
            EditingCalendarEvent.EventType = EditingCalendarEventEventType.ToString();
            EditingCalendarEvent.RelatedType = EditingCalendarEventRelatedType.ToString();
            EditingCalendarEvent.Visibility = EditingCalendarEventVisibility.ToString();

            // Set RelatedId based on RelatedType
            if (EditingCalendarEventRelatedType == RelatedType.PROJECT)
            {
                EditingCalendarEvent.RelatedId = SelectedEditProject.FirstOrDefault()?.Id.ToString();
            }
            else if (EditingCalendarEventRelatedType == RelatedType.TASK)
            {
                EditingCalendarEvent.RelatedId = SelectedEditProjectTask.FirstOrDefault()?.Id;
            }
            else
            {
                EditingCalendarEvent.RelatedId = null;
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

    // Validation methods
    private bool ValidateCreateCalendarEvent()
    {
        CreateCalendarEventValidationErrorKey = null;
        CreateFieldErrors.Clear();

        bool isValid = true;

        // Required: Title
        if (string.IsNullOrWhiteSpace(NewCalendarEvent?.Title))
        {
            CreateFieldErrors["Title"] = L["TitleRequired"];
            CreateCalendarEventValidationErrorKey = "TitleRequired";
            isValid = false;
        }

        // Required: EventType
        // EventType is enum, already set

        // Required: RelatedType
        // RelatedType is enum, already set

        // Required: RelatedId if RelatedType is PROJECT or TASK
        if (NewCalendarEventRelatedType == RelatedType.PROJECT)
        {
            if (SelectedNewProject == null || SelectedNewProject.Count == 0)
            {
                CreateFieldErrors["RelatedId"] = L["ProjectRequired"];
                if (isValid)
                {
                    CreateCalendarEventValidationErrorKey = "ProjectRequired";
                }
                isValid = false;
            }
        }
        else if (NewCalendarEventRelatedType == RelatedType.TASK)
        {
            if (SelectedNewProjectTask == null || SelectedNewProjectTask.Count == 0)
            {
                CreateFieldErrors["RelatedId"] = L["ProjectTaskRequired"];
                if (isValid)
                {
                    CreateCalendarEventValidationErrorKey = "ProjectTaskRequired";
                }
                isValid = false;
            }
        }

        // Required: Visibility
        // Visibility is enum, already set

        return isValid;
    }

    private bool ValidateEditCalendarEvent()
    {
        EditCalendarEventValidationErrorKey = null;
        EditFieldErrors.Clear();

        bool isValid = true;

        // Required: Title
        if (string.IsNullOrWhiteSpace(EditingCalendarEvent?.Title))
        {
            EditFieldErrors["Title"] = L["TitleRequired"];
            EditCalendarEventValidationErrorKey = "TitleRequired";
            isValid = false;
        }

        // Required: EventType
        // EventType is enum, already set

        // Required: RelatedType
        // RelatedType is enum, already set

        // Required: RelatedId if RelatedType is PROJECT or TASK
        if (EditingCalendarEventRelatedType == RelatedType.PROJECT)
        {
            if (SelectedEditProject == null || SelectedEditProject.Count == 0)
            {
                EditFieldErrors["RelatedId"] = L["ProjectRequired"];
                if (isValid)
                {
                    EditCalendarEventValidationErrorKey = "ProjectRequired";
                }
                isValid = false;
            }
        }
        else if (EditingCalendarEventRelatedType == RelatedType.TASK)
        {
            if (SelectedEditProjectTask == null || SelectedEditProjectTask.Count == 0)
            {
                EditFieldErrors["RelatedId"] = L["ProjectTaskRequired"];
                if (isValid)
                {
                    EditCalendarEventValidationErrorKey = "ProjectTaskRequired";
                }
                isValid = false;
            }
        }

        // Required: Visibility
        // Visibility is enum, already set

        return isValid;
    }

    // Lookup methods
    private async Task GetProjectCollectionLookupAsync(string? newValue = null)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task<List<LookupDto<Guid>>> GetProjectCollectionLookupAsync(IReadOnlyList<LookupDto<Guid>> dbset, string filter, CancellationToken token)
    {
        ProjectsCollection = (await ProjectTasksAppService.GetProjectLookupAsync(new LookupRequestDto { Filter = filter })).Items;
        return ProjectsCollection.ToList();
    }

    private async Task GetProjectTaskCollectionLookupAsync(string? newValue = null)
    {
        var input = new GetProjectTasksInput
        {
            FilterText = newValue,
            MaxResultCount = 20,
            SkipCount = 0,
        };

        var result = await ProjectTasksAppService.GetListAsync(input);
        ProjectTasksCollection = result.Items
            .Select(x => new ProjectTaskSelectItem
            {
                Id = x.ProjectTask.Code,
                DisplayName = $"{x.ProjectTask.Code} - {x.ProjectTask.Title}",
            })
            .ToList();
    }

    private async Task<List<ProjectTaskSelectItem>> GetProjectTaskCollectionLookupAsync(IReadOnlyList<ProjectTaskSelectItem> dbset, string filter, CancellationToken token)
    {
        var input = new GetProjectTasksInput
        {
            FilterText = filter,
            MaxResultCount = 20,
            SkipCount = 0,
        };

        var result = await ProjectTasksAppService.GetListAsync(input);
        ProjectTasksCollection = result.Items
            .Select(x => new ProjectTaskSelectItem
            {
                Id = x.ProjectTask.Code,
                DisplayName = $"{x.ProjectTask.Code} - {x.ProjectTask.Title}",
            })
            .ToList();

        return ProjectTasksCollection.ToList();
    }

    // Select2 change handlers
    protected void OnNewProjectChanged()
    {
        CreateFieldErrors.Remove("RelatedId");
    }

    protected void OnNewProjectTaskChanged()
    {
        CreateFieldErrors.Remove("RelatedId");
    }

    protected void OnEditProjectChanged()
    {
        EditFieldErrors.Remove("RelatedId");
    }

    protected void OnEditProjectTaskChanged()
    {
        EditFieldErrors.Remove("RelatedId");
    }

    private async Task OnSchedulerItemInserted(SchedulerInsertedItem<CalendarEventDto> item)
    {
        try
        {
            Logger.LogInformation("OnSchedulerItemInserted - Item inserted: Title: {Title}, StartTime: {StartTime}, EndTime: {EndTime}",
                item.Item.Title, item.Item.StartTime, item.Item.EndTime);
            
            // Fill NewCalendarEvent with data from the inserted appointment
            NewCalendarEvent = new CalendarEventCreateDto
            {
                Title = item.Item.Title,
                Description = item.Item.Description ?? string.Empty,
                StartTime = item.Item.StartTime,
                EndTime = item.Item.EndTime,
                AllDay = item.Item.AllDay,
                EventType = item.Item.EventType,
                RelatedType = item.Item.RelatedType,
                Visibility = item.Item.Visibility
            };
            
            // Parse and set enum properties
            if (Enum.TryParse<EventType>(item.Item.EventType, out var eventType))
            {
                NewCalendarEventEventType = eventType;
            }
            else
            {
                NewCalendarEventEventType = EventType.MEETING;
            }
            
            if (Enum.TryParse<RelatedType>(item.Item.RelatedType, out var relatedType))
            {
                NewCalendarEventRelatedType = relatedType;
            }
            else
            {
                NewCalendarEventRelatedType = RelatedType.NONE;
            }
            
            if (Enum.TryParse<EventVisibility>(item.Item.Visibility, out var visibility))
            {
                NewCalendarEventVisibility = visibility;
            }
            else
            {
                NewCalendarEventVisibility = EventVisibility.PRIVATE;
            }
            
            // Clear selections
            SelectedNewProject = new List<LookupDto<Guid>>();
            SelectedNewProjectTask = new List<ProjectTaskSelectItem>();
            CreateFieldErrors.Clear();
            CreateCalendarEventValidationErrorKey = null;
            
            // Open Create modal
            await CreateCalendarEventModal.Show();
            // Rebuild toolbar after opening modal
            RebuildToolbar();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnSchedulerItemClicked(SchedulerItemClickedEventArgs<CalendarEventDto> args)
    {
        try
        {
            Logger.LogInformation("OnSchedulerItemClicked - Item clicked: Id: {Id}, Title: {Title}, StartTime: {StartTime}, EndTime: {EndTime}",
                args.Item.Id, args.Item.Title, args.Item.StartTime, args.Item.EndTime);
            
            // Get the clicked appointment
            var calendarEvent = args.Item;
            // Open Edit modal with the clicked calendar event
            await OpenEditCalendarEventModalAsync(calendarEvent);
            // Rebuild toolbar after modal operations
            RebuildToolbar();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "OnSchedulerItemClicked - Error: {Message}", ex.Message);
            await HandleErrorAsync(ex);
        }
    }

    /*
    private async Task OnSchedulerItemUpdated(SchedulerUpdatedItem<SchedulerAppointment> item)

    private async Task OnSchedulerItemUpdated(SchedulerUpdatedItem<SchedulerAppointment> item)
    {
        try
        {
            // TODO: Adjust property access based on actual SchedulerUpdatedItem structure
            // Example: var appointment = item.Item; or item.Appointment; or direct access
            if (Guid.TryParse(appointment.Id, out var eventId))
            {
                var existingEvent = await CalendarEventsAppService.GetAsync(eventId);
                var updateDto = ObjectMapper.Map<CalendarEventDto, CalendarEventUpdateDto>(existingEvent);
                
                updateDto.Title = appointment.Title;
                updateDto.Description = appointment.Description;
                updateDto.StartTime = appointment.Start;
                updateDto.EndTime = appointment.End;
                updateDto.AllDay = appointment.AllDay;

                await CalendarEventsAppService.UpdateAsync(eventId, updateDto);
                await GetCalendarEventsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnSchedulerItemRemoved(SchedulerUpdatedItem<SchedulerAppointment> item)
    {
        try
        {
            // TODO: Adjust property access based on actual SchedulerUpdatedItem structure
            if (Guid.TryParse(appointment.Id, out var eventId))
            {
                await CalendarEventsAppService.DeleteAsync(eventId);
                await GetCalendarEventsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnSchedulerItemDropped(SchedulerItemDroppedEventArgs<SchedulerAppointment> args)
    {
        try
        {
            // TODO: Adjust property access based on actual SchedulerItemDroppedEventArgs structure
            if (Guid.TryParse(appointment.Id, out var eventId))
            {
                var existingEvent = await CalendarEventsAppService.GetAsync(eventId);
                var updateDto = ObjectMapper.Map<CalendarEventDto, CalendarEventUpdateDto>(existingEvent);
                
                updateDto.StartTime = appointment.Start;
                updateDto.EndTime = appointment.End;

                await CalendarEventsAppService.UpdateAsync(eventId, updateDto);
                await GetCalendarEventsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    */
}