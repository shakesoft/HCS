using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Positions;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Controllers.Positions;

[RemoteService]
[Area("app")]
[ControllerName("Position")]
[Route("api/app/positions")]
public abstract class PositionControllerBase : AbpController
{
    protected IPositionsAppService _positionsAppService;

    public PositionControllerBase(IPositionsAppService positionsAppService)
    {
        _positionsAppService = positionsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<PositionDto>> GetListAsync(GetPositionsInput input)
    {
        return _positionsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<PositionDto> GetAsync(Guid id)
    {
        return _positionsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<PositionDto> CreateAsync(PositionCreateDto input)
    {
        return _positionsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<PositionDto> UpdateAsync(Guid id, PositionUpdateDto input)
    {
        return _positionsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _positionsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(PositionExcelDownloadDto input)
    {
        return _positionsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _positionsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> positionIds)
    {
        return _positionsAppService.DeleteByIdsAsync(positionIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetPositionsInput input)
    {
        return _positionsAppService.DeleteAllAsync(input);
    }
}