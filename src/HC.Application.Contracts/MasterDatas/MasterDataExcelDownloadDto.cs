using Volo.Abp.Application.Dtos;
using System;

namespace HC.MasterDatas;

public abstract class MasterDataExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public string? Type { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public int? SortOrderMin { get; set; }

    public int? SortOrderMax { get; set; }

    public bool? IsActive { get; set; }

    public MasterDataExcelDownloadDtoBase()
    {
    }
}