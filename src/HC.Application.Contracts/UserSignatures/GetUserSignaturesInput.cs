using Volo.Abp.Application.Dtos;
using System;

namespace HC.UserSignatures;

public abstract class GetUserSignaturesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public SignType? SignType { get; set; }

    public string? ProviderCode { get; set; }

    public string? TokenRef { get; set; }

    public string? SignatureImage { get; set; }

    public DateTime? ValidFromMin { get; set; }

    public DateTime? ValidFromMax { get; set; }

    public DateTime? ValidToMin { get; set; }

    public DateTime? ValidToMax { get; set; }

    public bool? IsActive { get; set; }

    public Guid? IdentityUserId { get; set; }

    public GetUserSignaturesInputBase()
    {
    }
}