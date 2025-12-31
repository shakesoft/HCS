using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Units;

public partial interface IUnitsAppService : IApplicationService
{
    Task<PagedResultDto<UnitDto>> GetListAsync(GetUnitsInput input);
    Task<UnitDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<UnitDto> CreateAsync(UnitCreateDto input);
    Task<UnitDto> UpdateAsync(Guid id, UnitUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(UnitExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> unitIds);
    Task DeleteAllAsync(GetUnitsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}