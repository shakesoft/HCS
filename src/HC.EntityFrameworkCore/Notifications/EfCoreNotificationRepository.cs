using HC.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace HC.Notifications;

public abstract class EfCoreNotificationRepositoryBase : EfCoreRepository<HCDbContext, Notification, Guid>
{
    public EfCoreNotificationRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filterText, title, content, sourceType, eventType, relatedType, relatedId, priority);
        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Notification>> GetListAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, title, content, sourceType, eventType, relatedType, relatedId, priority);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? NotificationConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetDbSetAsync()), filterText, title, content, sourceType, eventType, relatedType, relatedId, priority);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Notification> ApplyFilter(IQueryable<Notification> query, string? filterText = null, string? title = null, string? content = null, string? sourceType = null, string? eventType = null, string? relatedType = null, string? relatedId = null, string? priority = null)
    {
        return query
        .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Content!.Contains(filterText!) || e.RelatedId!.Contains(filterText!) || e.Priority!.Contains(filterText!) || e.SourceType!.Contains(filterText!) || e.RelatedType!.Contains(filterText!) || e.EventType!.Contains(filterText!))
        .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
        .WhereIf(!string.IsNullOrWhiteSpace(content), e => e.Content.Contains(content))
        .WhereIf(!string.IsNullOrWhiteSpace(sourceType), e => e.SourceType.Contains(sourceType))
        .WhereIf(!string.IsNullOrWhiteSpace(eventType), e => e.EventType.Contains(eventType))
        .WhereIf(!string.IsNullOrWhiteSpace(relatedType), e => e.RelatedType.Contains(relatedType))
        .WhereIf(!string.IsNullOrWhiteSpace(relatedId), e => e.RelatedId.Contains(relatedId)).WhereIf(!string.IsNullOrWhiteSpace(priority), e => e.Priority.Contains(priority));
    }
}