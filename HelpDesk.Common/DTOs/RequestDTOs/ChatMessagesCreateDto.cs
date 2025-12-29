using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class ChatMessagesCreateDto
    {
        public long ChatSessionId { get; set; }
        public int? SenderId { get; set; }
        public int SenderType { get; set; }
        public string? ChatMessage { get; set; }
        public int MessageType { get; set; }
        public int VisibleTo { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public IFormFile? File { get; set; }
    }

}

