using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HC.Notifications;

public abstract class NotificationCreateDtoBase
{
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Content { get; set; } = null!;
    [Required]
    public string SourceType { get; set; } = null!;
    [Required]
    public string EventType { get; set; } = null!;
    [Required]
    public string RelatedType { get; set; } = null!;
    public string? RelatedId { get; set; }

    [Required]
    public string Priority { get; set; } = null!;
}