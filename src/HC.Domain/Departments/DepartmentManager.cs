using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Departments;

public abstract class DepartmentManagerBase : DomainService
{
    protected IDepartmentRepository _departmentRepository;

    public DepartmentManagerBase(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public virtual async Task<Department> CreateAsync(Guid? leaderUserId, string code, string name, int level, int sortOrder, bool isActive, string? parentId = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), DepartmentConsts.CodeMaxLength, DepartmentConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var department = new Department(GuidGenerator.Create(), leaderUserId, code, name, level, sortOrder, isActive, parentId);
        return await _departmentRepository.InsertAsync(department);
    }

    public virtual async Task<Department> UpdateAsync(Guid id, Guid? leaderUserId, string code, string name, int level, int sortOrder, bool isActive, string? parentId = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), DepartmentConsts.CodeMaxLength, DepartmentConsts.CodeMinLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        var department = await _departmentRepository.GetAsync(id);
        department.LeaderUserId = leaderUserId;
        department.Code = code;
        department.Name = name;
        department.Level = level;
        department.SortOrder = sortOrder;
        department.IsActive = isActive;
        department.ParentId = parentId;
        department.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _departmentRepository.UpdateAsync(department);
    }
}