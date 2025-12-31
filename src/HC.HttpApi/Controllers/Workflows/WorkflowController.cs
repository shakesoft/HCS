using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Workflows;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.Workflows;

[RemoteService]
[Area("app")]
[ControllerName("Workflow")]
[Route("api/app/workflows")]
public abstract class WorkflowControllerBase : AbpController
{
    protected IWorkflowsAppService _workflowsAppService;

    public WorkflowControllerBase(IWorkflowsAppService workflowsAppService)
    {
        _workflowsAppService = workflowsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<WorkflowDto>> GetListAsync(GetWorkflowsInput input)
    {
        return _workflowsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<WorkflowDto> GetAsync(Guid id)
    {
        return _workflowsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<WorkflowDto> CreateAsync(WorkflowCreateDto input)
    {
        return _workflowsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<WorkflowDto> UpdateAsync(Guid id, WorkflowUpdateDto input)
    {
        return _workflowsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _workflowsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowExcelDownloadDto input)
    {
        return _workflowsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _workflowsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> workflowIds)
    {
        return _workflowsAppService.DeleteByIdsAsync(workflowIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetWorkflowsInput input)
    {
        return _workflowsAppService.DeleteAllAsync(input);
    }
}