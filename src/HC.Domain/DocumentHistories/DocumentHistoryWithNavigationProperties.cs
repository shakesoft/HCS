using HC.Documents;
using Volo.Abp.Identity;
using Volo.Abp.Identity;
using System;
using System.Collections.Generic;
using HC.DocumentHistories;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryWithNavigationPropertiesBase
{
    public DocumentHistory DocumentHistory { get; set; } = null!;
    public Document Document { get; set; } = null!;
    public IdentityUser? FromUser { get; set; }

    public IdentityUser ToUser { get; set; } = null!;
}