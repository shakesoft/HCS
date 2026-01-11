using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace HC.CalendarEvents;

public abstract class CalendarEventManagerBase : DomainService
{
    protected ICalendarEventRepository _calendarEventRepository;

    public CalendarEventManagerBase(ICalendarEventRepository calendarEventRepository)
    {
        _calendarEventRepository = calendarEventRepository;
    }

    public virtual async Task<CalendarEvent> CreateAsync(string title, DateTime startTime, DateTime endTime, bool allDay, string eventType, string relatedType, string visibility, string? description = null, string? location = null, string? relatedId = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNullOrWhiteSpace(eventType, nameof(eventType));
        Check.NotNullOrWhiteSpace(relatedType, nameof(relatedType));
        Check.NotNullOrWhiteSpace(visibility, nameof(visibility));
        var calendarEvent = new CalendarEvent(GuidGenerator.Create(), title, startTime, endTime, allDay, eventType, relatedType, visibility, description, location, relatedId);
        return await _calendarEventRepository.InsertAsync(calendarEvent);
    }

    public virtual async Task<CalendarEvent> UpdateAsync(Guid id, string title, DateTime startTime, DateTime endTime, bool allDay, string eventType, string relatedType, string visibility, string? description = null, string? location = null, string? relatedId = null, [CanBeNull] string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(title, nameof(title));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNullOrWhiteSpace(eventType, nameof(eventType));
        Check.NotNullOrWhiteSpace(relatedType, nameof(relatedType));
        Check.NotNullOrWhiteSpace(visibility, nameof(visibility));
        var calendarEvent = await _calendarEventRepository.GetAsync(id);
        calendarEvent.Title = title;
        calendarEvent.StartTime = startTime;
        calendarEvent.EndTime = endTime;
        calendarEvent.AllDay = allDay;
        calendarEvent.EventType = eventType;
        calendarEvent.RelatedType = relatedType;
        calendarEvent.Visibility = visibility;
        calendarEvent.Description = description;
        calendarEvent.Location = location;
        calendarEvent.RelatedId = relatedId;
        calendarEvent.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await _calendarEventRepository.UpdateAsync(calendarEvent);
    }
}