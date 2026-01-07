using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.NotificationReceivers;

namespace HC.Controllers.NotificationReceivers;

[RemoteService]
[Area("app")]
[ControllerName("NotificationReceiver")]
[Route("api/app/notification-receivers")]
public class NotificationReceiverController : NotificationReceiverControllerBase
{
    public NotificationReceiverController(INotificationReceiversAppService notificationReceiversAppService) : base(notificationReceiversAppService)
    {
    }
}