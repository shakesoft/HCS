using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Positions;

namespace HC.Controllers.Positions;

[RemoteService]
[Area("app")]
[ControllerName("Position")]
[Route("api/app/positions")]
public class PositionController : PositionControllerBase, IPositionsAppService
{
    public PositionController(IPositionsAppService positionsAppService) : base(positionsAppService)
    {
    }
}