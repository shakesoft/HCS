using Volo.Abp.Application.Dtos;
using System;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public ParticipantResponse? ResponseStatus { get; set; }

    public bool? Notified { get; set; }

    public Guid? CalendarEventId { get; set; }

    public Guid? IdentityUserId { get; set; }

    public CalendarEventParticipantExcelDownloadDtoBase()
    {
    }
}