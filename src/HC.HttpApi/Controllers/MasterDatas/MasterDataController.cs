using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.MasterDatas;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.MasterDatas;

[RemoteService]
[Area("app")]
[ControllerName("MasterData")]
[Route("api/app/master-datas")]
public abstract class MasterDataControllerBase : AbpController
{
    protected IMasterDatasAppService _masterDatasAppService;

    public MasterDataControllerBase(IMasterDatasAppService masterDatasAppService)
    {
        _masterDatasAppService = masterDatasAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<MasterDataDto>> GetListAsync(GetMasterDatasInput input)
    {
        return _masterDatasAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MasterDataDto> GetAsync(Guid id)
    {
        return _masterDatasAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<MasterDataDto> CreateAsync(MasterDataCreateDto input)
    {
        return _masterDatasAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MasterDataDto> UpdateAsync(Guid id, MasterDataUpdateDto input)
    {
        return _masterDatasAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _masterDatasAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(MasterDataExcelDownloadDto input)
    {
        return _masterDatasAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _masterDatasAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> masterdataIds)
    {
        return _masterDatasAppService.DeleteByIdsAsync(masterdataIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetMasterDatasInput input)
    {
        return _masterDatasAppService.DeleteAllAsync(input);
    }
}