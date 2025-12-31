using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.WorkflowDefinitions;

public partial interface IWorkflowDefinitionsAppService : IApplicationService
{
    Task<PagedResultDto<WorkflowDefinitionDto>> GetListAsync(GetWorkflowDefinitionsInput input);
    Task<WorkflowDefinitionDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<WorkflowDefinitionDto> CreateAsync(WorkflowDefinitionCreateDto input);
    Task<WorkflowDefinitionDto> UpdateAsync(Guid id, WorkflowDefinitionUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowDefinitionExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> workflowdefinitionIds);
    Task DeleteAllAsync(GetWorkflowDefinitionsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}