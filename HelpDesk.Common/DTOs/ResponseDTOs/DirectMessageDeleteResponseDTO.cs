namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class DirectMessageDeleteResponseDTO
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public bool IsDeleted { get; set; }

        public int AffectedId { get; set; }

        public string Message { get; set; } = string.Empty;

    }
}