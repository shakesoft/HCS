using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.MasterDatas;

public partial interface IMasterDatasAppService : IApplicationService
{
    Task<PagedResultDto<MasterDataDto>> GetListAsync(GetMasterDatasInput input);
    Task<MasterDataDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<MasterDataDto> CreateAsync(MasterDataCreateDto input);
    Task<MasterDataDto> UpdateAsync(Guid id, MasterDataUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(MasterDataExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> masterdataIds);
    Task DeleteAllAsync(GetMasterDatasInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}