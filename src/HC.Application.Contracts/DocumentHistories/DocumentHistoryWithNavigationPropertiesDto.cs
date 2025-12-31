using HC.Documents;
using Volo.Abp.Identity;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.DocumentHistories;

public abstract class DocumentHistoryWithNavigationPropertiesDtoBase
{
    public DocumentHistoryDto DocumentHistory { get; set; } = null!;
    public DocumentDto Document { get; set; } = null!;
    public IdentityUserDto? FromUser { get; set; }

    public IdentityUserDto ToUser { get; set; } = null!;
}