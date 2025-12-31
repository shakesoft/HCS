using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Departments;

namespace HC.Controllers.Departments;

[RemoteService]
[Area("app")]
[ControllerName("Department")]
[Route("api/app/departments")]
public class DepartmentController : DepartmentControllerBase, IDepartmentsAppService
{
    public DepartmentController(IDepartmentsAppService departmentsAppService) : base(departmentsAppService)
    {
    }
}