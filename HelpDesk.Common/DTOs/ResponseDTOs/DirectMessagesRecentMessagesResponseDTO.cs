namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class DirectMessagesRecentMessagesResponseDTO
    {
        public long Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverFirstName { get; set; } = string.Empty;

        public string ReceiverLastName { get; set; } = string.Empty;

        public string? DirectMessage { get; set; }

        public int MessageType { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? MessageReadAt { get; set; }

        public DateTimeOffset? MessageUpdatedAt { get; set; }

        public int UnreadCount { get; set; }

        public string? AvatarUrl { get; set; }
    }
}