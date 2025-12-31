using HC.Workflows;
using HC.Units;
using HC.MasterDatas;
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

namespace HC.Documents;

public abstract class EfCoreDocumentRepositoryBase : EfCoreRepository<HCDbContext, Document, Guid>
{
    public EfCoreDocumentRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null, Guid? fieldId = null, Guid? unitId = null, Guid? workflowId = null, Guid? statusId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, no, title, type, urgencyLevel, secrecyLevel, currentStatus, completedTimeMin, completedTimeMax, fieldId, unitId, workflowId, statusId);
        var ids = query.Select(x => x.Document.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<DocumentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return (await GetDbSetAsync()).Where(b => b.Id == id).Select(document => new DocumentWithNavigationProperties { Document = document, Field = dbContext.Set<MasterData>().FirstOrDefault(c => c.Id == document.FieldId), Unit = dbContext.Set<Unit>().FirstOrDefault(c => c.Id == document.UnitId), Workflow = dbContext.Set<Workflow>().FirstOrDefault(c => c.Id == document.WorkflowId), Status = dbContext.Set<MasterData>().FirstOrDefault(c => c.Id == document.StatusId) }).FirstOrDefault();
    }

    public virtual async Task<List<DocumentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null, Guid? fieldId = null, Guid? unitId = null, Guid? workflowId = null, Guid? statusId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, no, title, type, urgencyLevel, secrecyLevel, currentStatus, completedTimeMin, completedTimeMax, fieldId, unitId, workflowId, statusId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DocumentConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<DocumentWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from document in (await GetDbSetAsync())
               join field in (await GetDbContextAsync()).Set<MasterData>() on document.FieldId equals field.Id into masterDatas
               from field in masterDatas.DefaultIfEmpty()
               join unit in (await GetDbContextAsync()).Set<Unit>() on document.UnitId equals unit.Id into units
               from unit in units.DefaultIfEmpty()
               join workflow in (await GetDbContextAsync()).Set<Workflow>() on document.WorkflowId equals workflow.Id into workflows
               from workflow in workflows.DefaultIfEmpty()
               join status in (await GetDbContextAsync()).Set<MasterData>() on document.StatusId equals status.Id into masterDatas1
               from status in masterDatas1.DefaultIfEmpty()
               select new DocumentWithNavigationProperties
               {
                   Document = document,
                   Field = field,
                   Unit = unit,
                   Workflow = workflow,
                   Status = status
               };
    }

    protected virtual IQueryable<DocumentWithNavigationProperties> ApplyFilter(IQueryable<DocumentWithNavigationProperties> query, string? filterText, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null, Guid? fieldId = null, Guid? unitId = null, Guid? workflowId = null, Guid? statusId = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Document.No!.Contains(filterText!) || e.Document.Title!.Contains(filterText!) || e.Document.Type!.Contains(filterText!) || e.Document.UrgencyLevel!.Contains(filterText!) || e.Document.SecrecyLevel!.Contains(filterText!) || e.Document.CurrentStatus!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(no), e => e.Document.No.Contains(no)).WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Document.Title.Contains(title)).WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Document.Type.Contains(type)).WhereIf(!string.IsNullOrWhiteSpace(urgencyLevel), e => e.Document.UrgencyLevel.Contains(urgencyLevel)).WhereIf(!string.IsNullOrWhiteSpace(secrecyLevel), e => e.Document.SecrecyLevel.Contains(secrecyLevel)).WhereIf(!string.IsNullOrWhiteSpace(currentStatus), e => e.Document.CurrentStatus.Contains(currentStatus)).WhereIf(completedTimeMin.HasValue, e => e.Document.CompletedTime >= completedTimeMin!.Value).WhereIf(completedTimeMax.HasValue, e => e.Document.CompletedTime <= completedTimeMax!.Value).WhereIf(fieldId != null && fieldId != Guid.Empty, e => e.Field != null && e.Field.Id == fieldId).WhereIf(unitId != null && unitId != Guid.Empty, e => e.Unit != null && e.Unit.Id == unitId).WhereIf(workflowId != null && workflowId != Guid.Empty, e => e.Workflow != null && e.Workflow.Id == workflowId).WhereIf(statusId != null && statusId != Guid.Empty, e => e.Status != null && e.Status.Id == statusId);
    }

    public virtual async Task<List<Document>> GetListAsync(string? filterText = null, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, no, title, type, urgencyLevel, secrecyLevel, currentStatus, completedTimeMin, completedTimeMax);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? DocumentConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null, Guid? fieldId = null, Guid? unitId = null, Guid? workflowId = null, Guid? statusId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, no, title, type, urgencyLevel, secrecyLevel, currentStatus, completedTimeMin, completedTimeMax, fieldId, unitId, workflowId, statusId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Document> ApplyFilter(IQueryable<Document> query, string? filterText = null, string? no = null, string? title = null, string? type = null, string? urgencyLevel = null, string? secrecyLevel = null, string? currentStatus = null, DateTime? completedTimeMin = null, DateTime? completedTimeMax = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.No!.Contains(filterText!) || e.Title!.Contains(filterText!) || e.Type!.Contains(filterText!) || e.UrgencyLevel!.Contains(filterText!) || e.SecrecyLevel!.Contains(filterText!) || e.CurrentStatus!.Contains(filterText!)).WhereIf(!string.IsNullOrWhiteSpace(no), e => e.No.Contains(no)).WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title)).WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Type.Contains(type)).WhereIf(!string.IsNullOrWhiteSpace(urgencyLevel), e => e.UrgencyLevel.Contains(urgencyLevel)).WhereIf(!string.IsNullOrWhiteSpace(secrecyLevel), e => e.SecrecyLevel.Contains(secrecyLevel)).WhereIf(!string.IsNullOrWhiteSpace(currentStatus), e => e.CurrentStatus.Contains(currentStatus)).WhereIf(completedTimeMin.HasValue, e => e.CompletedTime >= completedTimeMin!.Value).WhereIf(completedTimeMax.HasValue, e => e.CompletedTime <= completedTimeMax!.Value);
    }
}