using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using HC.EntityFrameworkCore;

namespace HC.CalendarEvents;

public abstract class EfCoreCalendarEventRepositoryBase : EfCoreRepository<HCDbContext, CalendarEvent, Guid>
{
    public EfCoreCalendarEventRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? title = null, string? description = null, DateTime? startTimeMin = null, DateTime? startTimeMax = null, DateTime? endTimeMin = null, DateTime? endTimeMax = null, bool? allDay = null, EventType? eventType = null, string? location = null, RelatedType? relatedType = null, string? relatedId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filterText, title, description, startTimeMin, startTimeMax, endTimeMin, endTimeMax, allDay, eventType, location, relatedType, relatedId);
        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<CalendarEvent>> GetListAsync(string? filterText = null, string? title = null, string? description = null, DateTime? startTimeMin = null, DateTime? startTimeMax = null, DateTime? endTimeMin = null, DateTime? endTimeMax = null, bool? allDay = null, EventType? eventType = null, string? location = null, RelatedType? relatedType = null, string? relatedId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, title, description, startTimeMin, startTimeMax, endTimeMin, endTimeMax, allDay, eventType, location, relatedType, relatedId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CalendarEventConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? title = null, string? description = null, DateTime? startTimeMin = null, DateTime? startTimeMax = null, DateTime? endTimeMin = null, DateTime? endTimeMax = null, bool? allDay = null, EventType? eventType = null, string? location = null, RelatedType? relatedType = null, string? relatedId = null, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, title, description, startTimeMin, startTimeMax, endTimeMin, endTimeMax, allDay, eventType, location, relatedType, relatedId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<CalendarEvent> ApplyFilter(IQueryable<CalendarEvent> query, string? filterText = null, string? title = null, string? description = null, DateTime? startTimeMin = null, DateTime? startTimeMax = null, DateTime? endTimeMin = null, DateTime? endTimeMax = null, bool? allDay = null, EventType? eventType = null, string? location = null, RelatedType? relatedType = null, string? relatedId = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Description!.Contains(filterText!) || e.Location!.Contains(filterText!) || e.RelatedId!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title)).WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description)).WhereIf(startTimeMin.HasValue, e => e.StartTime >= startTimeMin!.Value).WhereIf(startTimeMax.HasValue, e => e.StartTime <= startTimeMax!.Value).WhereIf(endTimeMin.HasValue, e => e.EndTime >= endTimeMin!.Value).WhereIf(endTimeMax.HasValue, e => e.EndTime <= endTimeMax!.Value).WhereIf(allDay.HasValue, e => e.AllDay == allDay).WhereIf(eventType.HasValue, e => e.EventType == eventType).WhereIf(!string.IsNullOrWhiteSpace(location), e => e.Location.Contains(location)).WhereIf(relatedType.HasValue, e => e.RelatedType == relatedType).WhereIf(!string.IsNullOrWhiteSpace(relatedId), e => e.RelatedId.Contains(relatedId));
    }
}