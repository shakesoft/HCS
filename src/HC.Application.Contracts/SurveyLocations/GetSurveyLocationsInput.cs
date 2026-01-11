using Volo.Abp.Application.Dtos;
using System;

namespace HC.SurveyLocations;

public abstract class GetSurveyLocationsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public GetSurveyLocationsInputBase()
    {
    }
}