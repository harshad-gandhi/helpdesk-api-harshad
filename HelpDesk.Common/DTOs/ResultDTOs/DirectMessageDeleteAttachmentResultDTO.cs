namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class DirectMessageDeleteAttachmentResultDTO
    {
        public int AffectedId { get; set; }

        public int ResultCode { get; set; }

        public string? FilePath { get; set; }
    }
}