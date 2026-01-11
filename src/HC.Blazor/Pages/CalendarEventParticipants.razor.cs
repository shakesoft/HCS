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
using HC.CalendarEventParticipants;
using HC.Permissions;
using HC.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;

namespace HC.Blazor.Pages;

public partial class CalendarEventParticipants
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    public DataGrid<CalendarEventParticipantWithNavigationPropertiesDto> DataGridRef { get; set; }

    private IReadOnlyList<CalendarEventParticipantWithNavigationPropertiesDto> CalendarEventParticipantList { get; set; }

    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private bool CanCreateCalendarEventParticipant { get; set; }

    private bool CanEditCalendarEventParticipant { get; set; }

    private bool CanDeleteCalendarEventParticipant { get; set; }

    private CalendarEventParticipantCreateDto NewCalendarEventParticipant { get; set; }

    private Validations NewCalendarEventParticipantValidations { get; set; } = new();
    private CalendarEventParticipantUpdateDto EditingCalendarEventParticipant { get; set; }

    private Validations EditingCalendarEventParticipantValidations { get; set; } = new();
    private Guid EditingCalendarEventParticipantId { get; set; }

    private Modal CreateCalendarEventParticipantModal { get; set; } = new();
    private Modal EditCalendarEventParticipantModal { get; set; } = new();
    private GetCalendarEventParticipantsInput Filter { get; set; }

    private DataGridEntityActionsColumn<CalendarEventParticipantWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();

    protected string SelectedCreateTab = "calendarEventParticipant-create-tab";
    protected string SelectedEditTab = "calendarEventParticipant-edit-tab";
    private CalendarEventParticipantWithNavigationPropertiesDto? SelectedCalendarEventParticipant;

    private IReadOnlyList<LookupDto<Guid>> CalendarEventsCollection { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
    private List<CalendarEventParticipantWithNavigationPropertiesDto> SelectedCalendarEventParticipants { get; set; } = new();
    private bool AllCalendarEventParticipantsSelected { get; set; }

    public CalendarEventParticipants()
    {
        NewCalendarEventParticipant = new CalendarEventParticipantCreateDto();
        EditingCalendarEventParticipant = new CalendarEventParticipantUpdateDto();
        Filter = new GetCalendarEventParticipantsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        CalendarEventParticipantList = new List<CalendarEventParticipantWithNavigationPropertiesDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetCalendarEventCollectionLookupAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["CalendarEventParticipants"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => {
            await DownloadAsExcelAsync();
        }, IconName.Download);
        Toolbar.AddButton(L["NewCalendarEventParticipant"], async () => {
            await OpenCreateCalendarEventParticipantModalAsync();
        }, IconName.Add, requiredPolicyName: HCPermissions.CalendarEventParticipants.Create);
        return ValueTask.CompletedTask;
    }

    private void ToggleDetails(CalendarEventParticipantWithNavigationPropertiesDto calendarEventParticipant)
    {
        DataGridRef.ToggleDetailRow(calendarEventParticipant, true);
    }

    private bool RowSelectableHandler(RowSelectableEventArgs<CalendarEventParticipantWithNavigationPropertiesDto> rowSelectableEventArgs) => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick && CanDeleteCalendarEventParticipant;

    private bool DetailRowTriggerHandler(DetailRowTriggerEventArgs<CalendarEventParticipantWithNavigationPropertiesDto> detailRowTriggerEventArgs)
    {
        detailRowTriggerEventArgs.Toggleable = false;
        detailRowTriggerEventArgs.DetailRowTriggerType = DetailRowTriggerType.Manual;
        return true;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateCalendarEventParticipant = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEventParticipants.Create);
        CanEditCalendarEventParticipant = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEventParticipants.Edit);
        CanDeleteCalendarEventParticipant = await AuthorizationService.IsGrantedAsync(HCPermissions.CalendarEventParticipants.Delete);
    }

    private async Task GetCalendarEventParticipantsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        var result = await CalendarEventParticipantsAppService.GetListAsync(Filter);
        CalendarEventParticipantList = result.Items;
        TotalCount = (int)result.TotalCount;
        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetCalendarEventParticipantsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await CalendarEventParticipantsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HC") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/calendar-event-participants/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ResponseStatus={HttpUtility.UrlEncode(Filter.ResponseStatus?.ToString())}&Notified={Filter.Notified}&CalendarEventId={Filter.CalendarEventId}&IdentityUserId={Filter.IdentityUserId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CalendarEventParticipantWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns.Where(c => c.SortDirection != SortDirection.Default).Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : "")).JoinAsString(",");
        CurrentPage = e.Page;
        await GetCalendarEventParticipantsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateCalendarEventParticipantModalAsync()
    {
        NewCalendarEventParticipant = new CalendarEventParticipantCreateDto
        {
            CalendarEventId = CalendarEventsCollection.Select(i => i.Id).FirstOrDefault(),
            IdentityUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
        };
        SelectedCreateTab = "calendarEventParticipant-create-tab";
        await NewCalendarEventParticipantValidations.ClearAll();
        await CreateCalendarEventParticipantModal.Show();
    }

    private async Task CloseCreateCalendarEventParticipantModalAsync()
    {
        NewCalendarEventParticipant = new CalendarEventParticipantCreateDto
        {
            CalendarEventId = CalendarEventsCollection.Select(i => i.Id).FirstOrDefault(),
            IdentityUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
        };
        await CreateCalendarEventParticipantModal.Hide();
    }

    private async Task OpenEditCalendarEventParticipantModalAsync(CalendarEventParticipantWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "calendarEventParticipant-edit-tab";
        var calendarEventParticipant = await CalendarEventParticipantsAppService.GetWithNavigationPropertiesAsync(input.CalendarEventParticipant.Id);
        EditingCalendarEventParticipantId = calendarEventParticipant.CalendarEventParticipant.Id;
        EditingCalendarEventParticipant = ObjectMapper.Map<CalendarEventParticipantDto, CalendarEventParticipantUpdateDto>(calendarEventParticipant.CalendarEventParticipant);
        await EditingCalendarEventParticipantValidations.ClearAll();
        await EditCalendarEventParticipantModal.Show();
    }

    private async Task DeleteCalendarEventParticipantAsync(CalendarEventParticipantWithNavigationPropertiesDto input)
    {
        try
        {
            await CalendarEventParticipantsAppService.DeleteAsync(input.CalendarEventParticipant.Id);
            await GetCalendarEventParticipantsAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateCalendarEventParticipantAsync()
    {
        try
        {
            if (await NewCalendarEventParticipantValidations.ValidateAll() == false)
            {
                return;
            }

            await CalendarEventParticipantsAppService.CreateAsync(NewCalendarEventParticipant);
            await GetCalendarEventParticipantsAsync();
            await CloseCreateCalendarEventParticipantModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditCalendarEventParticipantModalAsync()
    {
        await EditCalendarEventParticipantModal.Hide();
    }

    private async Task UpdateCalendarEventParticipantAsync()
    {
        try
        {
            if (await EditingCalendarEventParticipantValidations.ValidateAll() == false)
            {
                return;
            }

            await CalendarEventParticipantsAppService.UpdateAsync(EditingCalendarEventParticipantId, EditingCalendarEventParticipant);
            await GetCalendarEventParticipantsAsync();
            await EditCalendarEventParticipantModal.Hide();
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

    protected virtual async Task OnResponseStatusChangedAsync(string? responseStatus)
    {
        Filter.ResponseStatus = responseStatus;
        await SearchAsync();
    }

    protected virtual async Task OnNotifiedChangedAsync(bool? notified)
    {
        Filter.Notified = notified;
        await SearchAsync();
    }

    protected virtual async Task OnCalendarEventIdChangedAsync(Guid? calendarEventId)
    {
        Filter.CalendarEventId = calendarEventId;
        await SearchAsync();
    }

    protected virtual async Task OnIdentityUserIdChangedAsync(Guid? identityUserId)
    {
        Filter.IdentityUserId = identityUserId;
        await SearchAsync();
    }

    private async Task GetCalendarEventCollectionLookupAsync(string? newValue = null)
    {
        CalendarEventsCollection = (await CalendarEventParticipantsAppService.GetCalendarEventLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
    {
        IdentityUsersCollection = (await CalendarEventParticipantsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task SelectAllItems()
    {
        AllCalendarEventParticipantsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllCalendarEventParticipantsSelected = false;
        SelectedCalendarEventParticipants.Clear();
        return Task.CompletedTask;
    }

    private Task SelectedCalendarEventParticipantRowsChanged()
    {
        if (SelectedCalendarEventParticipants.Count != PageSize)
        {
            AllCalendarEventParticipantsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedCalendarEventParticipantsAsync()
    {
        var message = AllCalendarEventParticipantsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedCalendarEventParticipants.Count].Value;
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllCalendarEventParticipantsSelected)
        {
            await CalendarEventParticipantsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await CalendarEventParticipantsAppService.DeleteByIdsAsync(SelectedCalendarEventParticipants.Select(x => x.CalendarEventParticipant.Id).ToList());
        }

        SelectedCalendarEventParticipants.Clear();
        AllCalendarEventParticipantsSelected = false;
        await GetCalendarEventParticipantsAsync();
    }
}