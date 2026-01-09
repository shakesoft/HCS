using System;

namespace HC.UserSignatures;

public abstract class UserSignatureExcelDtoBase
{
    public SignType SignType { get; set; } = SignType.ELECTRONIC;
    public string ProviderCode { get; set; } = null!;
    public string? TokenRef { get; set; }

    public string SignatureImage { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; }
}