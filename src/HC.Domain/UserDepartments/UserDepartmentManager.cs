using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.UserDepartments;

public abstract class UserDepartmentManagerBase : DomainService
{
    protected IUserDepartmentRepository _userDepartmentRepository;

    public UserDepartmentManagerBase(IUserDepartmentRepository userDepartmentRepository)
    {
        _userDepartmentRepository = userDepartmentRepository;
    }

    public virtual async Task<UserDepartment> CreateAsync(Guid departmentId, Guid userId, bool isPrimary, bool isActive)
    {
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(userId, nameof(userId));
        var userDepartment = new UserDepartment(GuidGenerator.Create(), departmentId, userId, isPrimary, isActive);
        return await _userDepartmentRepository.InsertAsync(userDepartment);
    }

    public virtual async Task<UserDepartment> UpdateAsync(Guid id, Guid departmentId, Guid userId, bool isPrimary, bool isActive, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(userId, nameof(userId));
        var userDepartment = await _userDepartmentRepository.GetAsync(id);
        userDepartment.DepartmentId = departmentId;
        userDepartment.UserId = userId;
        userDepartment.IsPrimary = isPrimary;
        userDepartment.IsActive = isActive;
        userDepartment.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _userDepartmentRepository.UpdateAsync(userDepartment);
    }
}