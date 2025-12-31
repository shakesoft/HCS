using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.DocumentFiles;

namespace HC.Controllers.DocumentFiles;

[RemoteService]
[Area("app")]
[ControllerName("DocumentFile")]
[Route("api/app/document-files")]
public class DocumentFileController : DocumentFileControllerBase, IDocumentFilesAppService
{
    public DocumentFileController(IDocumentFilesAppService documentFilesAppService) : base(documentFilesAppService)
    {
    }
}