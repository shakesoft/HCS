using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.CalendarEvents;

namespace HC.Controllers.CalendarEvents;

[RemoteService]
[Area("app")]
[ControllerName("CalendarEvent")]
[Route("api/app/calendar-events")]
public class CalendarEventController : CalendarEventControllerBase, ICalendarEventsAppService
{
    public CalendarEventController(ICalendarEventsAppService calendarEventsAppService) : base(calendarEventsAppService)
    {
    }
}