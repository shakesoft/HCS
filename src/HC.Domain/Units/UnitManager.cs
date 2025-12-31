using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Units;

public abstract class UnitManagerBase : DomainService
{
    protected IUnitRepository _unitRepository;

    public UnitManagerBase(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public virtual async Task<Unit> CreateAsync(string code, string name, int sortOrder, bool isActive)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), UnitConsts.CodeMaxLength, UnitConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var unit = new Unit(GuidGenerator.Create(), code, name, sortOrder, isActive);
        return await _unitRepository.InsertAsync(unit);
    }

    public virtual async Task<Unit> UpdateAsync(Guid id, string code, string name, int sortOrder, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), UnitConsts.CodeMaxLength, UnitConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var unit = await _unitRepository.GetAsync(id);
        unit.Code = code;
        unit.Name = name;
        unit.SortOrder = sortOrder;
        unit.IsActive = isActive;
        unit.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _unitRepository.UpdateAsync(unit);
    }
}