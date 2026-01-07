using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.CalendarEventParticipants;

namespace HC.Controllers.CalendarEventParticipants;

[RemoteService]
[Area("app")]
[ControllerName("CalendarEventParticipant")]
[Route("api/app/calendar-event-participants")]
public class CalendarEventParticipantController : CalendarEventParticipantControllerBase, ICalendarEventParticipantsAppService
{
    public CalendarEventParticipantController(ICalendarEventParticipantsAppService calendarEventParticipantsAppService) : base(calendarEventParticipantsAppService)
    {
    }
}