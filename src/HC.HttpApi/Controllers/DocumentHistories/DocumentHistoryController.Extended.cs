using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.DocumentHistories;

namespace HC.Controllers.DocumentHistories;

[RemoteService]
[Area("app")]
[ControllerName("DocumentHistory")]
[Route("api/app/document-histories")]
public class DocumentHistoryController : DocumentHistoryControllerBase, IDocumentHistoriesAppService
{
    public DocumentHistoryController(IDocumentHistoriesAppService documentHistoriesAppService) : base(documentHistoriesAppService)
    {
    }
}