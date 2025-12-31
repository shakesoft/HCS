using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.WorkflowDefinitions;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.WorkflowDefinitions;

[RemoteService]
[Area("app")]
[ControllerName("WorkflowDefinition")]
[Route("api/app/workflow-definitions")]
public abstract class WorkflowDefinitionControllerBase : AbpController
{
    protected IWorkflowDefinitionsAppService _workflowDefinitionsAppService;

    public WorkflowDefinitionControllerBase(IWorkflowDefinitionsAppService workflowDefinitionsAppService)
    {
        _workflowDefinitionsAppService = workflowDefinitionsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<WorkflowDefinitionDto>> GetListAsync(GetWorkflowDefinitionsInput input)
    {
        return _workflowDefinitionsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<WorkflowDefinitionDto> GetAsync(Guid id)
    {
        return _workflowDefinitionsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<WorkflowDefinitionDto> CreateAsync(WorkflowDefinitionCreateDto input)
    {
        return _workflowDefinitionsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<WorkflowDefinitionDto> UpdateAsync(Guid id, WorkflowDefinitionUpdateDto input)
    {
        return _workflowDefinitionsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _workflowDefinitionsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowDefinitionExcelDownloadDto input)
    {
        return _workflowDefinitionsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _workflowDefinitionsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> workflowdefinitionIds)
    {
        return _workflowDefinitionsAppService.DeleteByIdsAsync(workflowdefinitionIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetWorkflowDefinitionsInput input)
    {
        return _workflowDefinitionsAppService.DeleteAllAsync(input);
    }
}