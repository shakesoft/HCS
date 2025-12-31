using Volo.Abp.Identity;
using HC.WorkflowTemplates;
using HC.WorkflowStepTemplates;
using HC.Workflows;
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

namespace HC.WorkflowStepAssignments;

public abstract class EfCoreWorkflowStepAssignmentRepositoryBase : EfCoreRepository<HCDbContext, WorkflowStepAssignment, Guid>
{
    public EfCoreWorkflowStepAssignmentRepositoryBase(IDbContextProvider<HCDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public virtual async Task DeleteAllAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, isPrimary, isActive, workflowId, stepId, templateId, defaultUserId);
        var ids = query.Select(x => x.WorkflowStepAssignment.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<WorkflowStepAssignmentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        return (await GetDbSetAsync()).Where(b => b.Id == id).Select(workflowStepAssignment => new WorkflowStepAssignmentWithNavigationProperties { WorkflowStepAssignment = workflowStepAssignment, Workflow = dbContext.Set<Workflow>().FirstOrDefault(c => c.Id == workflowStepAssignment.WorkflowId), Step = dbContext.Set<WorkflowStepTemplate>().FirstOrDefault(c => c.Id == workflowStepAssignment.StepId), Template = dbContext.Set<WorkflowTemplate>().FirstOrDefault(c => c.Id == workflowStepAssignment.TemplateId), DefaultUser = dbContext.Set<IdentityUser>().FirstOrDefault(c => c.Id == workflowStepAssignment.DefaultUserId) }).FirstOrDefault();
    }

    public virtual async Task<List<WorkflowStepAssignmentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, isPrimary, isActive, workflowId, stepId, templateId, defaultUserId);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? WorkflowStepAssignmentConsts.GetDefaultSorting(true) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<WorkflowStepAssignmentWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
    {
        return from workflowStepAssignment in (await GetDbSetAsync())
               join workflow in (await GetDbContextAsync()).Set<Workflow>() on workflowStepAssignment.WorkflowId equals workflow.Id into workflows
               from workflow in workflows.DefaultIfEmpty()
               join step in (await GetDbContextAsync()).Set<WorkflowStepTemplate>() on workflowStepAssignment.StepId equals step.Id into workflowStepTemplates
               from step in workflowStepTemplates.DefaultIfEmpty()
               join template in (await GetDbContextAsync()).Set<WorkflowTemplate>() on workflowStepAssignment.TemplateId equals template.Id into workflowTemplates
               from template in workflowTemplates.DefaultIfEmpty()
               join defaultUser in (await GetDbContextAsync()).Set<IdentityUser>() on workflowStepAssignment.DefaultUserId equals defaultUser.Id into identityUsers
               from defaultUser in identityUsers.DefaultIfEmpty()
               select new WorkflowStepAssignmentWithNavigationProperties
               {
                   WorkflowStepAssignment = workflowStepAssignment,
                   Workflow = workflow,
                   Step = step,
                   Template = template,
                   DefaultUser = defaultUser
               };
    }

    protected virtual IQueryable<WorkflowStepAssignmentWithNavigationProperties> ApplyFilter(IQueryable<WorkflowStepAssignmentWithNavigationProperties> query, string? filterText, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true).WhereIf(isPrimary.HasValue, e => e.WorkflowStepAssignment.IsPrimary == isPrimary).WhereIf(isActive.HasValue, e => e.WorkflowStepAssignment.IsActive == isActive).WhereIf(workflowId != null && workflowId != Guid.Empty, e => e.Workflow != null && e.Workflow.Id == workflowId).WhereIf(stepId != null && stepId != Guid.Empty, e => e.Step != null && e.Step.Id == stepId).WhereIf(templateId != null && templateId != Guid.Empty, e => e.Template != null && e.Template.Id == templateId).WhereIf(defaultUserId != null && defaultUserId != Guid.Empty, e => e.DefaultUser != null && e.DefaultUser.Id == defaultUserId);
    }

    public virtual async Task<List<WorkflowStepAssignment>> GetListAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()), filterText, isPrimary, isActive);
        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? WorkflowStepAssignmentConsts.GetDefaultSorting(false) : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, bool? isPrimary = null, bool? isActive = null, Guid? workflowId = null, Guid? stepId = null, Guid? templateId = null, Guid? defaultUserId = null, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryForNavigationPropertiesAsync();
        query = ApplyFilter(query, filterText, isPrimary, isActive, workflowId, stepId, templateId, defaultUserId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<WorkflowStepAssignment> ApplyFilter(IQueryable<WorkflowStepAssignment> query, string? filterText = null, bool? isPrimary = null, bool? isActive = null)
    {
        return query.WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true).WhereIf(isPrimary.HasValue, e => e.IsPrimary == isPrimary).WhereIf(isActive.HasValue, e => e.IsActive == isActive);
    }
}