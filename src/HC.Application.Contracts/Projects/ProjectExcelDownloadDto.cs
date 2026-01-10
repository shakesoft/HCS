using HC.Projects;
using Volo.Abp.Application.Dtos;
using System;

namespace HC.Projects;

public abstract class ProjectExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDateMin { get; set; }

    public DateTime? StartDateMax { get; set; }

    public DateTime? EndDateMin { get; set; }

    public DateTime? EndDateMax { get; set; }

    public ProjectStatus? Status { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public ProjectExcelDownloadDtoBase()
    {
    }
}