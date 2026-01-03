using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using HC.Notifications;

namespace HC.Controllers.Notifications;

[RemoteService]
[Area("app")]
[ControllerName("Notification")]
[Route("api/app/notifications")]
public class NotificationController : NotificationControllerBase, INotificationsAppService
{
    public NotificationController(INotificationsAppService notificationsAppService) : base(notificationsAppService)
    {
    }
}