namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class DirectMessageGetResponseDTO
    {
        public long Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string? DirectMessage { get; set; }

        public int MessageType { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? MessageReadAt { get; set; }

        public DateTimeOffset? MessageUpdatedAt { get; set; }

        public string? FilePath { get; set; }

        public string? OriginalFileName { get; set; }

    }
}