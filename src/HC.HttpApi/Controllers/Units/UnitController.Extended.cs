using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Units;

namespace HC.Controllers.Units;

[RemoteService]
[Area("app")]
[ControllerName("Unit")]
[Route("api/app/units")]
public class UnitController : UnitControllerBase, IUnitsAppService
{
    public UnitController(IUnitsAppService unitsAppService) : base(unitsAppService)
    {
    }
}