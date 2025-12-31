using Volo.Abp.Application.Dtos;
using System;

namespace HC.Positions;

public abstract class PositionExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? SignOrderMin { get; set; }

    public int? SignOrderMax { get; set; }

    public bool? IsActive { get; set; }

    public PositionExcelDownloadDtoBase()
    {
    }
}