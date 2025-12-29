namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class DirectMessageSendResultDTO
    {

        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string? DirectMessage { get; set; }

        public int MessageType { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? MessageReadAt { get; set; }

        public DateTimeOffset? MessageUpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int AffectedId { get; set; }

        public int ResultCode { get; set; }

        public string? FilePath { get; set; }
    }
}