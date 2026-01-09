using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.SignatureSettings;

public abstract class SignatureSettingCreateDtoBase
{
    [Required]
    public string ProviderCode { get; set; } = null!;
    [Required]
    public ProviderType ProviderType { get; set; } = ProviderType.HSM;
    [Required]
    public string ApiEndpoint { get; set; } = null!;
    public int ApiTimeout { get; set; }

    [Required]
    public SignType DefaultSignType { get; set; } = SignType.ELECTRONIC;
    public bool AllowElectronicSign { get; set; }

    public bool AllowDigitalSign { get; set; }

    public bool RequireOtp { get; set; }

    public int SignWidth { get; set; }

    public int SignHeight { get; set; }

    [Required]
    public string SignedFileSuffix { get; set; } = null!;
    public bool KeepOriginalFile { get; set; }

    public bool OverwriteSignedFile { get; set; }

    public bool EnableSignLog { get; set; }

    public bool IsActive { get; set; }
}