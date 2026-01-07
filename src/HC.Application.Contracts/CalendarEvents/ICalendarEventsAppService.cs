using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using HC.Shared;

namespace HC.CalendarEvents;

public partial interface ICalendarEventsAppService : IApplicationService
{
    Task<PagedResultDto<CalendarEventDto>> GetListAsync(GetCalendarEventsInput input);
    Task<CalendarEventDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<CalendarEventDto> CreateAsync(CalendarEventCreateDto input);
    Task<CalendarEventDto> UpdateAsync(Guid id, CalendarEventUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(CalendarEventExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> calendareventIds);
    Task DeleteAllAsync(GetCalendarEventsInput input);
    Task<HC.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}