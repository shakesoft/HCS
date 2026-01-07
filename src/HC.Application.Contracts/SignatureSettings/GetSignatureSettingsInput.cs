using Volo.Abp.Application.Dtos;
using System;

namespace HC.SignatureSettings;

public abstract class GetSignatureSettingsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? ProviderCode { get; set; }

    public string? ProviderType { get; set; }

    public string? ApiEndpoint { get; set; }

    public int? ApiTimeoutMin { get; set; }

    public int? ApiTimeoutMax { get; set; }

    public string? DefaultSignType { get; set; }

    public bool? AllowElectronicSign { get; set; }

    public bool? AllowDigitalSign { get; set; }

    public bool? RequireOtp { get; set; }

    public int? SignWidthMin { get; set; }

    public int? SignWidthMax { get; set; }

    public int? SignHeightMin { get; set; }

    public int? SignHeightMax { get; set; }

    public string? SignedFileSuffix { get; set; }

    public bool? KeepOriginalFile { get; set; }

    public bool? OverwriteSignedFile { get; set; }

    public bool? EnableSignLog { get; set; }

    public bool? IsActive { get; set; }

    public GetSignatureSettingsInputBase()
    {
    }
}