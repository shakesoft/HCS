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
    private IReadOnlyList<Appointment> SchedulerEventList { get; set; } = new List<Appointment>();

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

    // Appointment class for Scheduler
    public class Appointment
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }

        public Appointment()
        {
        }

        public Appointment(string title, string description, DateTime start, DateTime end, bool allDay = false)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Description = description;
            Start = start;
            End = end;
            AllDay = allDay;
        }
    }

    // Scheduler properties
    private DateOnly SelectedSchedulerDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    private SchedulerView SelectedSchedulerView { get; set; } = SchedulerView.Month;
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
        // Ensure SelectedSchedulerDate is first day of month if in Month view
        if (SelectedSchedulerView == SchedulerView.Month)
        {
            var firstDayOfMonth = new DateOnly(SelectedSchedulerDate.Year, SelectedSchedulerDate.Month, 1);
            if (SelectedSchedulerDate != firstDayOfMonth)
            {
                SelectedSchedulerDate = firstDayOfMonth;
            }
        }
        
        await SetPermissionsAsync();
        InitializeSchedulerLocalizers();
        try
        {
            await GetProjectCollectionLookupAsync();
            // Load calendar events on initialization
            await GetCalendarEventsAsync();
        }
        catch (Exception ex)
        {
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
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            // Ensure data is loaded for both views
            if (!CalendarEventList.Any())
            {
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
        IsListView = !IsListView;
        RebuildToolbar();
        
        await GetCalendarEventsAsync();
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
                // For Calendar view, load all events (max 1000 due to server limit)
                calendarFilter = new GetCalendarEventsInput
                {
                    MaxResultCount = 1000, // Maximum allowed by server
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
            
            // For Calendar view, load all events in batches if needed (max 1000 per request)
            if (!IsListView)
            {
                var allEvents = new List<CalendarEventDto>();
                int skipCount = 0;
                const int maxBatchSize = 1000;
                
                while (true)
                {
                    calendarFilter.MaxResultCount = maxBatchSize;
                    calendarFilter.SkipCount = skipCount;
                    
                    var result = await CalendarEventsAppService.GetListAsync(calendarFilter);
                    var batchItems = result.Items ?? new List<CalendarEventDto>();
                    
                    if (!batchItems.Any())
                        break;
                    
                    allEvents.AddRange(batchItems);
                    
                    // If we got less than maxBatchSize, we've reached the end
                    if (batchItems.Count < maxBatchSize)
                        break;
                    
                    skipCount += maxBatchSize;
                    
                    // Safety limit: don't load more than 10000 events total
                    if (allEvents.Count >= 10000)
                        break;
                }
                
                CalendarEventList = allEvents;
                TotalCount = allEvents.Count;
                
                // Convert CalendarEventDto to Appointment for Scheduler
                var isMonthView = SelectedSchedulerView == SchedulerView.Month;
                
                if (isMonthView)
                {
                    // For Month view, split multi-day events into single-day appointments
                    var appointments = new List<Appointment>();
                    
                    foreach (var evt in allEvents)
                    {
                        var startDate = evt.StartTime.Date;
                        var endDate = evt.EndTime.Date;
                        
                        // If event spans multiple days, create one appointment per day
                        if (endDate > startDate)
                        {
                            var currentDate = startDate;
                            while (currentDate <= endDate)
                            {
                                appointments.Add(new Appointment(
                                    title: evt.Title ?? string.Empty,
                                    description: evt.Description ?? string.Empty,
                                    start: currentDate,
                                    end: currentDate.AddHours(23).AddMinutes(59).AddSeconds(59),
                                    allDay: true
                                )
                                {
                                    Id = $"{evt.Id}_{currentDate:yyyyMMdd}"
                                });
                                
                                currentDate = currentDate.AddDays(1);
                            }
                        }
                        else
                        {
                            // Single day event
                            appointments.Add(new Appointment(
                                title: evt.Title ?? string.Empty,
                                description: evt.Description ?? string.Empty,
                                start: startDate,
                                end: startDate.AddHours(23).AddMinutes(59).AddSeconds(59),
                                allDay: true
                            )
                            {
                                Id = evt.Id.ToString()
                            });
                        }
                    }
                    
                    SchedulerEventList = appointments;
                }
                else
                {
                    // For other views, use original logic
                    SchedulerEventList = allEvents.Select(evt =>
                    {
                        var startTime = evt.StartTime;
                        var endTime = evt.EndTime;
                        var allDay = evt.AllDay;
                        
                        if (evt.AllDay)
                        {
                            // AllDay events: set to full day
                            startTime = evt.StartTime.Date;
                            var endDate = evt.EndTime.Date;
                            if (endDate > evt.StartTime.Date)
                            {
                                endTime = endDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                            }
                            else
                            {
                                endTime = evt.StartTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                            }
                        }
                        else if (evt.StartTime >= evt.EndTime)
                        {
                            // Events with StartTime >= EndTime: set EndTime to StartTime + 1 hour (minimum duration)
                            endTime = evt.StartTime.AddHours(1);
                        }
                        
                        return new Appointment(
                            title: evt.Title ?? string.Empty,
                            description: evt.Description ?? string.Empty,
                            start: startTime,
                            end: endTime,
                            allDay: allDay
                        )
                        {
                            Id = evt.Id.ToString()
                        };
                    }).ToList();
                }
            }
            else
            {
                // For List view, use single request with pagination
                var result = await CalendarEventsAppService.GetListAsync(calendarFilter);
                CalendarEventList = result.Items ?? new List<CalendarEventDto>();
                TotalCount = (int)(result?.TotalCount ?? 0);
            }
            
            if (CalendarEventList.Any())
            {
                var firstEvent = CalendarEventList.First();
                
                // For Calendar view, set SelectedSchedulerDate appropriately based on view type
                if (!IsListView && CalendarEventList.Any())
                {
                    if (SelectedSchedulerView == SchedulerView.Month)
                    {
                        // For Month view, ensure SelectedSchedulerDate is first day of month
                        var firstDayOfMonth = new DateOnly(SelectedSchedulerDate.Year, SelectedSchedulerDate.Month, 1);
                        if (SelectedSchedulerDate != firstDayOfMonth)
                        {
                            SelectedSchedulerDate = firstDayOfMonth;
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                    else
                    {
                        // For other views, set to first event's date
                        var firstEventDate = DateOnly.FromDateTime(firstEvent.StartTime);
                        if (SelectedSchedulerDate != firstEventDate)
                        {
                            SelectedSchedulerDate = firstEventDate;
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                }
            }
            
            await ClearSelection();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        CalendarEventList = new List<CalendarEventDto>();
        SchedulerEventList = new List<Appointment>();
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
            
            // Refresh scheduler if in Calendar view
            if (!IsListView)
            {
                await InvokeAsync(StateHasChanged);
            }
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
            
            // Refresh scheduler if in Calendar view
            if (!IsListView)
            {
                await InvokeAsync(StateHasChanged);
            }
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
            
            // Refresh scheduler if in Calendar view
            if (!IsListView)
            {
                await InvokeAsync(StateHasChanged);
            }
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
        
        // Refresh scheduler if in Calendar view
        if (!IsListView)
        {
            await InvokeAsync(StateHasChanged);
        }
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

    private async Task OnSchedulerItemInserted(SchedulerInsertedItem<Appointment> item)
    {
        try
        {
            // Find original CalendarEventDto if exists (for existing events), otherwise use defaults for new events
            CalendarEventDto? originalEvent = null;
            // Handle split event IDs (format: {eventId}_{date}) for Month view
            var eventIdString = item.Item.Id.Contains('_') ? item.Item.Id.Split('_')[0] : item.Item.Id;
            if (Guid.TryParse(eventIdString, out var eventId))
            {
                originalEvent = CalendarEventList.FirstOrDefault(e => e.Id == eventId);
            }
            
            NewCalendarEvent = new CalendarEventCreateDto
            {
                Title = item.Item.Title,
                Description = item.Item.Description ?? string.Empty,
                StartTime = item.Item.Start,
                EndTime = item.Item.End,
                AllDay = item.Item.AllDay,
                EventType = originalEvent?.EventType ?? EventType.MEETING.ToString(),
                RelatedType = originalEvent?.RelatedType ?? RelatedType.NONE.ToString(),
                Visibility = originalEvent?.Visibility ?? EventVisibility.PRIVATE.ToString()
            };
            
            if (originalEvent != null && Enum.TryParse<EventType>(originalEvent.EventType, out var eventType))
            {
                NewCalendarEventEventType = eventType;
            }
            else
            {
                NewCalendarEventEventType = EventType.MEETING;
            }
            
            if (originalEvent != null && Enum.TryParse<RelatedType>(originalEvent.RelatedType, out var relatedType))
            {
                NewCalendarEventRelatedType = relatedType;
            }
            else
            {
                NewCalendarEventRelatedType = RelatedType.NONE;
            }
            
            if (originalEvent != null && Enum.TryParse<EventVisibility>(originalEvent.Visibility, out var visibility))
            {
                NewCalendarEventVisibility = visibility;
            }
            else
            {
                NewCalendarEventVisibility = EventVisibility.PRIVATE;
            }
            
            SelectedNewProject = new List<LookupDto<Guid>>();
            SelectedNewProjectTask = new List<ProjectTaskSelectItem>();
            CreateFieldErrors.Clear();
            CreateCalendarEventValidationErrorKey = null;
            
            await CreateCalendarEventModal.Show();
            RebuildToolbar();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnSchedulerItemClicked(SchedulerItemClickedEventArgs<Appointment> args)
    {
        try
        {
            Logger.LogInformation("OnSchedulerItemClicked - Item clicked: Id: {Id}, Title: {Title}, Start: {Start}, End: {End}",
                args.Item.Id, args.Item.Title, args.Item.Start, args.Item.End);
            
            // Convert Appointment back to CalendarEventDto by finding it in CalendarEventList
            if (Guid.TryParse(args.Item.Id, out var eventId))
            {
                var calendarEvent = CalendarEventList.FirstOrDefault(e => e.Id == eventId);
                if (calendarEvent == null)
                {
                    Logger.LogWarning("OnSchedulerItemClicked - CalendarEvent not found for Id: {Id}, fetching from API", args.Item.Id);
                    // Try to fetch from API if not in list
                    calendarEvent = await CalendarEventsAppService.GetAsync(eventId);
                }
                
                // Open Edit modal with the clicked calendar event
                await OpenEditCalendarEventModalAsync(calendarEvent);
            }
            else
            {
                Logger.LogError("OnSchedulerItemClicked - Invalid Id format: {Id}", args.Item.Id);
            }
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
}