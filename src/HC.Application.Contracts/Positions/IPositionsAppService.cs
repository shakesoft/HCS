using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Positions;

public partial interface IPositionsAppService : IApplicationService
{
    Task<PagedResultDto<PositionDto>> GetListAsync(GetPositionsInput input);
    Task<PositionDto> GetAsync(Guid id);
    Task<PagedResultDto<LookupDto<Guid>>> GetPositionLookupAsync(LookupRequestDto input);
    Task DeleteAsync(Guid id);
    Task<PositionDto> CreateAsync(PositionCreateDto input);
    Task<PositionDto> UpdateAsync(Guid id, PositionUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(PositionExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> positionIds);
    Task DeleteAllAsync(GetPositionsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}