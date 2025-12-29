namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class ChatShortCutResponseDTO
    {
        public int? Id { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public required string ShortCutKey { get; set; }

        public required string ShortCutMessage { get; set; }

        public bool IsPublic { get; set; }
    }
}