using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.DocumentAssignments;

namespace HC.Controllers.DocumentAssignments;

[RemoteService]
[Area("app")]
[ControllerName("DocumentAssignment")]
[Route("api/app/document-assignments")]
public class DocumentAssignmentController : DocumentAssignmentControllerBase, IDocumentAssignmentsAppService
{
    public DocumentAssignmentController(IDocumentAssignmentsAppService documentAssignmentsAppService) : base(documentAssignmentsAppService)
    {
    }
}