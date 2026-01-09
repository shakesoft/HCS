using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.UserSignatures;

namespace HC.UserSignatures;

public abstract class UserSignatureWithNavigationPropertiesBase
{
    public UserSignature UserSignature { get; set; } = null!;
    public IdentityUser IdentityUser { get; set; } = null!;
}