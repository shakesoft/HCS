using System;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantExcelDtoBase
{
    public ParticipantResponse ResponseStatus { get; set; } = ParticipantResponse.INVITED;
    public bool Notified { get; set; }
}