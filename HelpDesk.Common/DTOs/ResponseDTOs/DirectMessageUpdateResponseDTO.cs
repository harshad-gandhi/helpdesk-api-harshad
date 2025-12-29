namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class DirectMessageUpdateResponseDTO
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public string? DirectMessage { get; set; }

        public DateTimeOffset? MessageReadAt { get; set; }

        public DateTimeOffset? MessageUpdatedAt { get; set; }

        public int AffectedId { get; set; }

        public string? Message { get; set; }

    }
}