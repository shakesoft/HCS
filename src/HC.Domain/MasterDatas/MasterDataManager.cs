using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.MasterDatas;

public abstract class MasterDataManagerBase : DomainService
{
    protected IMasterDataRepository _masterDataRepository;

    public MasterDataManagerBase(IMasterDataRepository masterDataRepository)
    {
        _masterDataRepository = masterDataRepository;
    }

    public virtual async Task<MasterData> CreateAsync(string type, string code, string name, int sortOrder, bool isActive)
    {
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), MasterDataConsts.TypeMaxLength, MasterDataConsts.TypeMinLength);
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), MasterDataConsts.CodeMaxLength, MasterDataConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Range(sortOrder, nameof(sortOrder), MasterDataConsts.SortOrderMinLength, MasterDataConsts.SortOrderMaxLength);
        var masterData = new MasterData(GuidGenerator.Create(), type, code, name, sortOrder, isActive);
        return await _masterDataRepository.InsertAsync(masterData);
    }

    public virtual async Task<MasterData> UpdateAsync(Guid id, string type, string code, string name, int sortOrder, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), MasterDataConsts.TypeMaxLength, MasterDataConsts.TypeMinLength);
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), MasterDataConsts.CodeMaxLength, MasterDataConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Range(sortOrder, nameof(sortOrder), MasterDataConsts.SortOrderMinLength, MasterDataConsts.SortOrderMaxLength);
        var masterData = await _masterDataRepository.GetAsync(id);
        masterData.Type = type;
        masterData.Code = code;
        masterData.Name = name;
        masterData.SortOrder = sortOrder;
        masterData.IsActive = isActive;
        masterData.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _masterDataRepository.UpdateAsync(masterData);
    }
}