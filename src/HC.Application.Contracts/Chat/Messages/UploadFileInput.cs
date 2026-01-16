using System;

namespace HC.Chat.Messages;

// Note: For HttpApi layer, use IFormFile from Microsoft.AspNetCore.Http
// For Application layer, use FileContent as byte[]
public class UploadFileInput
{
    public byte[] FileContent { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public Guid? ConversationId { get; set; } // Optional: pre-upload before sending message
}
