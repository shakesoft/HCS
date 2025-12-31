using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Workflows;

public partial interface IWorkflowsAppService : IApplicationService
{
    Task<PagedResultDto<WorkflowDto>> GetListAsync(GetWorkflowsInput input);
    Task<WorkflowDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<WorkflowDto> CreateAsync(WorkflowCreateDto input);
    Task<WorkflowDto> UpdateAsync(Guid id, WorkflowUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(WorkflowExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> workflowIds);
    Task DeleteAllAsync(GetWorkflowsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}