using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.UserDepartments;

namespace HC.Controllers.UserDepartments;

[RemoteService]
[Area("app")]
[ControllerName("UserDepartment")]
[Route("api/app/user-departments")]
public class UserDepartmentController : UserDepartmentControllerBase, IUserDepartmentsAppService
{
    public UserDepartmentController(IUserDepartmentsAppService userDepartmentsAppService) : base(userDepartmentsAppService)
    {
    }
}