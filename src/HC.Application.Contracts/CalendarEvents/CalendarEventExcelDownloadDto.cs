using Volo.Abp.Application.Dtos;
using System;

namespace HC.CalendarEvents;

public abstract class CalendarEventExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartTimeMin { get; set; }

    public DateTime? StartTimeMax { get; set; }

    public DateTime? EndTimeMin { get; set; }

    public DateTime? EndTimeMax { get; set; }

    public bool? AllDay { get; set; }

    public string? EventType { get; set; }

    public string? Location { get; set; }

    public string? RelatedType { get; set; }

    public string? RelatedId { get; set; }

    public string? Visibility { get; set; }

    public CalendarEventExcelDownloadDtoBase()
    {
    }
}