using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.Notifications;

public partial interface INotificationsAppService : IApplicationService
{
    Task<PagedResultDto<NotificationDto>> GetListAsync(GetNotificationsInput input);
    Task<NotificationDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<NotificationDto> CreateAsync(NotificationCreateDto input);
    Task<NotificationDto> UpdateAsync(Guid id, NotificationUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> notificationIds);
    Task DeleteAllAsync(GetNotificationsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}