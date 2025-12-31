using Volo.Abp.Application.Dtos;
using System;

namespace HC.Departments;

public abstract class GetDepartmentsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? ParentId { get; set; }

    public int? LevelMin { get; set; }

    public int? LevelMax { get; set; }

    public int? SortOrderMin { get; set; }

    public int? SortOrderMax { get; set; }

    public bool? IsActive { get; set; }

    public Guid? LeaderUserId { get; set; }

    public GetDepartmentsInputBase()
    {
    }
}