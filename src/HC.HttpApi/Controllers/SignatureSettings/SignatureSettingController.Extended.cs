using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.SignatureSettings;

namespace HC.Controllers.SignatureSettings;

[RemoteService]
[Area("app")]
[ControllerName("SignatureSetting")]
[Route("api/app/signature-settings")]
public class SignatureSettingController : SignatureSettingControllerBase, ISignatureSettingsAppService
{
    public SignatureSettingController(ISignatureSettingsAppService signatureSettingsAppService) : base(signatureSettingsAppService)
    {
    }
}