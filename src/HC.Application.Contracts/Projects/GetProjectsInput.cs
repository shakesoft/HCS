using Volo.Abp.Application.Dtos;
using System;

namespace HC.Projects;

public abstract class GetProjectsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDateMin { get; set; }

    public DateTime? StartDateMax { get; set; }

    public DateTime? EndDateMin { get; set; }

    public DateTime? EndDateMax { get; set; }

    public string? Status { get; set; }

    public Guid? OwnerDepartmentId { get; set; }

    public GetProjectsInputBase()
    {
    }
}