using HC.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HC.NotificationReceivers;

public partial interface INotificationReceiversAppService : IApplicationService
{
    Task<PagedResultDto<NotificationReceiverWithNavigationPropertiesDto>> GetListAsync(GetNotificationReceiversInput input);
    Task<NotificationReceiverWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
    Task<NotificationReceiverDto> GetAsync(Guid id);
    Task<PagedResultDto<LookupDto<Guid>>> GetNotificationLookupAsync(LookupRequestDto input);
    Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input);
    Task DeleteAsync(Guid id);
    Task<NotificationReceiverDto> CreateAsync(NotificationReceiverCreateDto input);
    Task<NotificationReceiverDto> UpdateAsync(Guid id, NotificationReceiverUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(NotificationReceiverExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> notificationreceiverIds);
    Task DeleteAllAsync(GetNotificationReceiversInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    Task<PagedResultDto<NotificationReceiverWithNavigationPropertiesDto>> GetReadNotificationsAsync(GetUserNotificationsInput input);
    Task<PagedResultDto<NotificationReceiverWithNavigationPropertiesDto>> GetUnreadNotificationsAsync(GetUserNotificationsInput input);
}