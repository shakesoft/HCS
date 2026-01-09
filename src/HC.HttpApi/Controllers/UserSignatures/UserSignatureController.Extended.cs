using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.UserSignatures;

namespace HC.Controllers.UserSignatures;

[RemoteService]
[Area("app")]
[ControllerName("UserSignature")]
[Route("api/app/user-signatures")]
public class UserSignatureController : UserSignatureControllerBase, IUserSignaturesAppService
{
    public UserSignatureController(IUserSignaturesAppService userSignaturesAppService) : base(userSignaturesAppService)
    {
    }
}