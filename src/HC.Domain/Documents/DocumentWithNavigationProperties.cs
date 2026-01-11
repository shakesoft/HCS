using HC.MasterDatas;
using HC.Units;
using HC.Workflows;
using HC.MasterDatas;
using HC.MasterDatas;
using HC.MasterDatas;
using HC.MasterDatas;
using System;
using System.Collections.Generic;
using HC.Documents;

namespace HC.Documents;

public abstract class DocumentWithNavigationPropertiesBase
{
    public Document Document { get; set; } = null!;
    public MasterData? Field { get; set; }

    public Unit? Unit { get; set; }

    public Workflow? Workflow { get; set; }

    public MasterData? Status { get; set; }

    public MasterData Type { get; set; } = null!;
    public MasterData UrgencyLevel { get; set; } = null!;
    public MasterData SecrecyLevel { get; set; } = null!;
}