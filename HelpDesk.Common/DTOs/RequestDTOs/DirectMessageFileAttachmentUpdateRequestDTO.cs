using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class DirectMessageFileAttachmentUpdateRequestDTO
    {
        public int DirectMessageId { get; set; }

        public IFormFile? NewFile { get; set; }
    }
}