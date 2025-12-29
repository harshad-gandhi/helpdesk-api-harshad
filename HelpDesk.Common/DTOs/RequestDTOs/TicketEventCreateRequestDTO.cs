using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TicketEventCreateRequestDTO
    {
        public long TicketId { get; set; }
        public int EventType { get; set; }
        public int PerformerType { get; set; }
        public string? EventText { get; set; }
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; } = false;
        public int? CreatedBy { get; set; }

        public string? EmailMessageId { get; set; }

        public List<IFormFile>? Files { get; set; }

        public List<TicketEventAttachmentDto> Attachments { get; set; } = [];
    }

    public class TicketEventAttachmentDto
    {
        public string? Filename { get; set; }
        public string? OriginalFilename { get; set; }
        public string? FilePath { get; set; }
        public int MimeType { get; set; }
        public long FileSizeBytes { get; set; }
    }
}
