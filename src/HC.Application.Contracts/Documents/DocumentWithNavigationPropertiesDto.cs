using HC.MasterDatas;
using HC.Units;
using HC.Workflows;
using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace HC.Documents;

public abstract class DocumentWithNavigationPropertiesDtoBase
{
    public DocumentDto Document { get; set; } = null!;
    public MasterDataDto? Field { get; set; }

    public UnitDto? Unit { get; set; }

    public WorkflowDto? Workflow { get; set; }

    public MasterDataDto? Status { get; set; }

    public MasterDataDto Type { get; set; } = null!;
    public MasterDataDto UrgencyLevel { get; set; } = null!;
    public MasterDataDto SecrecyLevel { get; set; } = null!;
}