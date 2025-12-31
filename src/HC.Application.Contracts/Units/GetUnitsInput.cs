using Volo.Abp.Application.Dtos;
using System;

namespace HC.Units;

public abstract class GetUnitsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? SortOrderMin { get; set; }

    public int? SortOrderMax { get; set; }

    public bool? IsActive { get; set; }

    public GetUnitsInputBase()
    {
    }
}