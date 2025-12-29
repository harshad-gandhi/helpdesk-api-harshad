namespace HelpDesk.Common.DTOs.CommonDTOs;

public class ChatAttachmentDTO
{
    public long Id { get; set; }
    public long ChatMessageId { get; set; }
    public string FileName { get; set; }
    public string OriginalFileName { get; set; }
    public string FilePath { get; set; }
    public int MimeType { get; set; }
    public int FileSizeByte { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
