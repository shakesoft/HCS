using System.IO;

namespace HC.Chat.Messages;

public class FileDto
{
    public byte[] Content { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
