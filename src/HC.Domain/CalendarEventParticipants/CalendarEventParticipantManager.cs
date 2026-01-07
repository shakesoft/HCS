using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.CalendarEventParticipants;

public abstract class CalendarEventParticipantManagerBase : DomainService
{
    protected ICalendarEventParticipantRepository _calendarEventParticipantRepository;

    public CalendarEventParticipantManagerBase(ICalendarEventParticipantRepository calendarEventParticipantRepository)
    {
        _calendarEventParticipantRepository = calendarEventParticipantRepository;
    }

    public virtual async Task<CalendarEventParticipant> CreateAsync(Guid calendarEventId, Guid identityUserId, string responseStatus, bool notified)
    {
        Check.NotNull(calendarEventId, nameof(calendarEventId));
        Check.NotNull(identityUserId, nameof(identityUserId));
        Check.NotNullOrWhiteSpace(responseStatus, nameof(responseStatus));
        var calendarEventParticipant = new CalendarEventParticipant(GuidGenerator.Create(), calendarEventId, identityUserId, responseStatus, notified);
        return await _calendarEventParticipantRepository.InsertAsync(calendarEventParticipant);
    }

    public virtual async Task<CalendarEventParticipant> UpdateAsync(Guid id, Guid calendarEventId, Guid identityUserId, string responseStatus, bool notified, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(calendarEventId, nameof(calendarEventId));
        Check.NotNull(identityUserId, nameof(identityUserId));
        Check.NotNullOrWhiteSpace(responseStatus, nameof(responseStatus));
        var calendarEventParticipant = await _calendarEventParticipantRepository.GetAsync(id);
        calendarEventParticipant.CalendarEventId = calendarEventId;
        calendarEventParticipant.IdentityUserId = identityUserId;
        calendarEventParticipant.ResponseStatus = responseStatus;
        calendarEventParticipant.Notified = notified;
        calendarEventParticipant.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _calendarEventParticipantRepository.UpdateAsync(calendarEventParticipant);
    }
}