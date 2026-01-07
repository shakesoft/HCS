using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HC.CalendarEventParticipants;

public partial interface ICalendarEventParticipantRepository : IRepository<CalendarEventParticipant, Guid>
{
    Task DeleteAllAsync(string? filterText = null, string? responseStatus = null, bool? notified = null, Guid? calendarEventId = null, Guid? identityUserId = null, CancellationToken cancellationToken = default);
    Task<CalendarEventParticipantWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<CalendarEventParticipantWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? responseStatus = null, bool? notified = null, Guid? calendarEventId = null, Guid? identityUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<List<CalendarEventParticipant>> GetListAsync(string? filterText = null, string? responseStatus = null, bool? notified = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);
    Task<long> GetCountAsync(string? filterText = null, string? responseStatus = null, bool? notified = null, Guid? calendarEventId = null, Guid? identityUserId = null, CancellationToken cancellationToken = default);
}