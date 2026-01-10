using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.Projects;

public abstract class ProjectManagerBase : DomainService
{
    protected IProjectRepository _projectRepository;

    public ProjectManagerBase(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public virtual async Task<Project> CreateAsync(Guid? ownerDepartmentId, string code, string name, DateTime startDate, DateTime endDate, string status, string? description = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), ProjectConsts.CodeMaxLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), ProjectConsts.NameMaxLength);
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));
        Check.NotNullOrWhiteSpace(status, nameof(status));
        var project = new Project(GuidGenerator.Create(), ownerDepartmentId, code, name, startDate, endDate, status, description);
        return await _projectRepository.InsertAsync(project);
    }

    public virtual async Task<Project> UpdateAsync(Guid id, Guid? ownerDepartmentId, string code, string name, DateTime startDate, DateTime endDate, string status, string? description = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(code, nameof(code));
        Check.Length(code, nameof(code), ProjectConsts.CodeMaxLength);
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.Length(name, nameof(name), ProjectConsts.NameMaxLength);
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));
        Check.NotNullOrWhiteSpace(status, nameof(status));
        var project = await _projectRepository.GetAsync(id);
        project.OwnerDepartmentId = ownerDepartmentId;
        project.Code = code;
        project.Name = name;
        project.StartDate = startDate;
        project.EndDate = endDate;
        project.Status = status;
        project.Description = description;
        project.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _projectRepository.UpdateAsync(project);
    }
}