using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.MasterDatas;

namespace HC.Controllers.MasterDatas;

[RemoteService]
[Area("app")]
[ControllerName("MasterData")]
[Route("api/app/master-datas")]
public class MasterDataController : MasterDataControllerBase, IMasterDatasAppService
{
    public MasterDataController(IMasterDatasAppService masterDatasAppService) : base(masterDatasAppService)
    {
    }
}