using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.UserSignatures;

public abstract class UserSignatureCreateDtoBase
{
    [Required]
    public SignType SignType { get; set; } = SignType.ELECTRONIC;
    [Required]
    public string ProviderCode { get; set; } = null!;
    public string? TokenRef { get; set; }

    [Required]
    public string SignatureImage { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; }

    public Guid IdentityUserId { get; set; }
}