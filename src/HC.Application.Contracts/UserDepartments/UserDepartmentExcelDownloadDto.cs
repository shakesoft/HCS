using Volo.Abp.Application.Dtos;
using System;

namespace HC.UserDepartments;

public abstract class UserDepartmentExcelDownloadDtoBase
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }

    public bool? IsPrimary { get; set; }

    public bool? IsActive { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? UserId { get; set; }

    public UserDepartmentExcelDownloadDtoBase()
    {
    }
}