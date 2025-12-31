using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HC.Permissions;
using HC.Workflows;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using HC.Shared;

namespace HC.Workflows;

[RemoteService(IsEnabled = false)]
[Authorize(HCPermissions.Workflows.Default)]
public abstract class WorkflowsAppServiceBase : HCAppService
{
    protected IDistributedCache<WorkflowDownloadTokenCacheItem, string> _downloadTokenCache;
    protected IWorkflowRepository _workflowRepository;
    protected WorkflowManager _workflowManager;

    public WorkflowsAppServiceBase(IWorkflowRepository workflowRepository, WorkflowManager workflowManager, IDistributedCache<WorkflowDownloadTokenCacheItem, string> downloadTokenCache)
    {
        _downloadTokenCache = downloadTokenCache;
        _workflowRepository = workflowRepository;
        _workflowManager = workflowManager;
    }

    public virtual async Task<PagedResultDto<WorkflowDto>> GetListAsync(GetWorkflowsInput input)
    {
        var totalCount = await _workflowRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsActive);
        var items = await _workflowRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<WorkflowDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Workflow>, List<WorkflowDto>>(items)
        };
    }

    public virtual async Task<WorkflowDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Workflow, WorkflowDto>(await _workflowRepository.GetAsync(id));
    }

    [Authorize(HCPermissions.Workflows.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _workflowRepository.DeleteAsync(id);
    }

    [Authorize(HCPermissions.Workflows.Create)]
    public virtual async Task<WorkflowDto> CreateAsync(WorkflowCreateDto input)
    {
        var workflow = await _workflowManager.CreateAsync(input.Code, input.Name, input.IsActive, input.Description);
        return ObjectMapper.Map<Workflow, WorkflowDto>(workflow);
    }

    [Authorize(HCPermissions.Workflows.Edit)]
    public virtual async Task<WorkflowDto> UpdateAsync(Guid id, WorkflowUpdateDto input)
    {
        var workflow = await _workflowManager.UpdateAsync(id, input.Code, input.Name, input.IsActive, input.Description, input.ConcurrencyStamp);
        return ObjectMapper.Map<Workflow, WorkflowDto>(workflow);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowExcelDownloadDto input)
    {
        var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await _workflowRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsActive);
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Workflow>, List<WorkflowExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "Workflows.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HCPermissions.Workflows.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> workflowIds)
    {
        await _workflowRepository.DeleteManyAsync(workflowIds);
    }

    [Authorize(HCPermissions.Workflows.Delete)]
    public virtual async Task DeleteAllAsync(GetWorkflowsInput input)
    {
        await _workflowRepository.DeleteAllAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsActive);
    }

    public virtual async Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");
        await _downloadTokenCache.SetAsync(token, new WorkflowDownloadTokenCacheItem { Token = token }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) });
        return new HC.Shared.DownloadTokenResultDto
        {
            Token = token
        };
    }
}