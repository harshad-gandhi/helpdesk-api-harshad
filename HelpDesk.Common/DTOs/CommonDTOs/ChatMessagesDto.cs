namespace HelpDesk.Common.DTOs.CommonDTOs
{
    public class ChatMessagesDto
    {
        public long Id { get; set; }
        public long ChatSessionId { get; set; }
        public int? SenderId { get; set; }
        public int SenderType { get; set; }
        public string? ChatMessage { get; set; }
        public int MessageType { get; set; }
        public int VisibleTo { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public string? FilePath { get; set; }
        public string? OriginalFileName { get; set; }
        public int? MimeType { get; set; }
}

}

