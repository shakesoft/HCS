using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace HC.Chat.Messages;

public class MessageFile : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }
    public virtual Guid MessageId { get; protected set; }
    public virtual string FileName { get; protected set; }
    public virtual string FilePath { get; protected set; } // Blob storage path
    public virtual string ContentType { get; protected set; }
    public virtual long FileSize { get; protected set; }
    public virtual string FileExtension { get; protected set; }
    public virtual DateTime CreationTime { get; protected set; }
    public virtual Guid? CreatorId { get; protected set; }
    
    // Navigation
    public virtual Message Message { get; protected set; }
    
    protected MessageFile()
    {
    }
    
    public MessageFile(
        Guid id,
        Guid messageId,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        string fileExtension,
        Guid? creatorId = null,
        Guid? tenantId = null)
        : base(id)
    {
        MessageId = messageId;
        FileName = Check.NotNullOrWhiteSpace(fileName, nameof(fileName));
        FilePath = Check.NotNullOrWhiteSpace(filePath, nameof(filePath));
        ContentType = contentType;
        FileSize = fileSize;
        FileExtension = fileExtension;
        CreatorId = creatorId;
        TenantId = tenantId;
        CreationTime = DateTime.UtcNow;
    }
    
    public virtual void SetMessageId(Guid messageId)
    {
        MessageId = messageId;
    }
}
