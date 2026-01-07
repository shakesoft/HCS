using System;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}