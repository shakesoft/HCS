using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Positions;

public abstract class PositionManagerBase : DomainService
{
    protected IPositionRepository _positionRepository;

    public PositionManagerBase(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public virtual async Task<Position> CreateAsync(string code, string name, int signOrder, bool isActive)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), PositionConsts.CodeMaxLength, PositionConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Range(signOrder, nameof(signOrder), PositionConsts.SignOrderMinLength, PositionConsts.SignOrderMaxLength);
        var position = new Position(GuidGenerator.Create(), code, name, signOrder, isActive);
        return await _positionRepository.InsertAsync(position);
    }

    public virtual async Task<Position> UpdateAsync(Guid id, string code, string name, int signOrder, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), PositionConsts.CodeMaxLength, PositionConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Range(signOrder, nameof(signOrder), PositionConsts.SignOrderMinLength, PositionConsts.SignOrderMaxLength);
        var position = await _positionRepository.GetAsync(id);
        position.Code = code;
        position.Name = name;
        position.SignOrder = signOrder;
        position.IsActive = isActive;
        position.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _positionRepository.UpdateAsync(position);
    }
}