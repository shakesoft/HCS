using System;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantExcelDtoBase
{
    public string ResponseStatus { get; set; } = null!;
    public bool Notified { get; set; }
}