using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.UserSignatures;

public abstract class UserSignatureWithNavigationPropertiesDtoBase
{
    public UserSignatureDto UserSignature { get; set; } = null!;
    public IdentityUserDto IdentityUser { get; set; } = null!;
}