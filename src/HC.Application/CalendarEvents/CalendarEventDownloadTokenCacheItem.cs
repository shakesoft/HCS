using System;

namespace HC.CalendarEvents;

public abstract class CalendarEventDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}