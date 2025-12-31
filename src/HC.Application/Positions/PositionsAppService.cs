using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HC.Permissions;
using HC.Positions;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using HC.Shared;

namespace HC.Positions;

[RemoteService(IsEnabled = false)]
[Authorize(HCPermissions.Positions.Default)]
public abstract class PositionsAppServiceBase : HCAppService
{
    protected IDistributedCache<PositionDownloadTokenCacheItem, string> _downloadTokenCache;
    protected IPositionRepository _positionRepository;
    protected PositionManager _positionManager;

    public PositionsAppServiceBase(IPositionRepository positionRepository, PositionManager positionManager, IDistributedCache<PositionDownloadTokenCacheItem, string> downloadTokenCache)
    {
        _downloadTokenCache = downloadTokenCache;
        _positionRepository = positionRepository;
        _positionManager = positionManager;
    }

    public virtual async Task<PagedResultDto<PositionDto>> GetListAsync(GetPositionsInput input)
    {
        var totalCount = await _positionRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.SignOrderMin, input.SignOrderMax, input.IsActive);
        var items = await _positionRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.SignOrderMin, input.SignOrderMax, input.IsActive, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<PositionDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Position>, List<PositionDto>>(items)
        };
    }

    public virtual async Task<PositionDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Position, PositionDto>(await _positionRepository.GetAsync(id));
    }

    [Authorize(HCPermissions.Positions.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _positionRepository.DeleteAsync(id);
    }

    [Authorize(HCPermissions.Positions.Create)]
    public virtual async Task<PositionDto> CreateAsync(PositionCreateDto input)
    {
        var position = await _positionManager.CreateAsync(input.Code, input.Name, input.SignOrder, input.IsActive);
        return ObjectMapper.Map<Position, PositionDto>(position);
    }

    [Authorize(HCPermissions.Positions.Edit)]
    public virtual async Task<PositionDto> UpdateAsync(Guid id, PositionUpdateDto input)
    {
        var position = await _positionManager.UpdateAsync(id, input.Code, input.Name, input.SignOrder, input.IsActive, input.ConcurrencyStamp);
        return ObjectMapper.Map<Position, PositionDto>(position);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PositionExcelDownloadDto input)
    {
        var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await _positionRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.SignOrderMin, input.SignOrderMax, input.IsActive);
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Position>, List<PositionExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return new RemoteStreamContent(memoryStream, "Positions.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HCPermissions.Positions.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> positionIds)
    {
        await _positionRepository.DeleteManyAsync(positionIds);
    }

    [Authorize(HCPermissions.Positions.Delete)]
    public virtual async Task DeleteAllAsync(GetPositionsInput input)
    {
        await _positionRepository.DeleteAllAsync(input.FilterText, input.Code, input.Name, input.SignOrderMin, input.SignOrderMax, input.IsActive);
    }

    public virtual async Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");
        await _downloadTokenCache.SetAsync(token, new PositionDownloadTokenCacheItem { Token = token }, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) });
        return new HC.Shared.DownloadTokenResultDto
        {
            Token = token
        };
    }
}