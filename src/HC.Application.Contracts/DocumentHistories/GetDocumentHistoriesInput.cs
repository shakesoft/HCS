using Volo.Abp.Application.Dtos;
using System;

namespace HC.DocumentHistories;

public abstract class GetDocumentHistoriesInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Comment { get; set; }

    public string? Action { get; set; }

    public Guid? DocumentId { get; set; }

    public Guid? FromUser { get; set; }

    public Guid? ToUser { get; set; }

    public GetDocumentHistoriesInputBase()
    {
    }
}