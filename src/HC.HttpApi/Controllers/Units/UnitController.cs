using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Units;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.Units;

[RemoteService]
[Area("app")]
[ControllerName("Unit")]
[Route("api/app/units")]
public abstract class UnitControllerBase : AbpController
{
    protected IUnitsAppService _unitsAppService;

    public UnitControllerBase(IUnitsAppService unitsAppService)
    {
        _unitsAppService = unitsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<UnitDto>> GetListAsync(GetUnitsInput input)
    {
        return _unitsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<UnitDto> GetAsync(Guid id)
    {
        return _unitsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<UnitDto> CreateAsync(UnitCreateDto input)
    {
        return _unitsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<UnitDto> UpdateAsync(Guid id, UnitUpdateDto input)
    {
        return _unitsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _unitsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(UnitExcelDownloadDto input)
    {
        return _unitsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _unitsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> unitIds)
    {
        return _unitsAppService.DeleteByIdsAsync(unitIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetUnitsInput input)
    {
        return _unitsAppService.DeleteAllAsync(input);
    }
}