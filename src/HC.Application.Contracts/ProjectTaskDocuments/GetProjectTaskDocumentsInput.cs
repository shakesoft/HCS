using Volo.Abp.Application.Dtos;
using System;

namespace HC.ProjectTaskDocuments;

public abstract class GetProjectTaskDocumentsInputBase : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? DocumentPurpose { get; set; }

    public Guid? ProjectTaskId { get; set; }

    public Guid? DocumentId { get; set; }

    public GetProjectTaskDocumentsInputBase()
    {
    }
}