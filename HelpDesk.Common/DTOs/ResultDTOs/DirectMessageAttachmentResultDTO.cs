using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class DirectMessageAttachmentResultDTO
    {
        public int? Id { get; set; }

        public int DirectMessageId { get; set; }

        public required string OriginalFileName { get; set; }

        public required string FileName { get; set; }

        public required string FilePath { get; set; }

        public int MimeType { get; set; }

        public int FileSizeByte { get; set; }

        public bool IsDeleted { get; set; }

        public IFormFile? File { get; set; }
    }
}