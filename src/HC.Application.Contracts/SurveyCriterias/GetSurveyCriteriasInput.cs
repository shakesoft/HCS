using Volo.Abp.Application.Dtos;
using System;

namespace HC.SurveyCriterias;

public abstract class GetSurveyCriteriasInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public int? DisplayOrderMin { get; set; }

    public int? DisplayOrderMax { get; set; }

    public bool? IsActive { get; set; }

    public Guid? SurveyLocationId { get; set; }

    public GetSurveyCriteriasInputBase()
    {
    }
}