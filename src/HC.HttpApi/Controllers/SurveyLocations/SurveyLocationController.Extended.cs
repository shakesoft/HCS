using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.SurveyLocations;

namespace HC.Controllers.SurveyLocations;

[RemoteService]
[Area("app")]
[ControllerName("SurveyLocation")]
[Route("api/app/survey-locations")]
public class SurveyLocationController : SurveyLocationControllerBase, ISurveyLocationsAppService
{
    public SurveyLocationController(ISurveyLocationsAppService surveyLocationsAppService) : base(surveyLocationsAppService)
    {
    }
}