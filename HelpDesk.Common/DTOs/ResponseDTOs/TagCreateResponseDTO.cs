using HelpDesk.Common.Enums;

namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class TagCreateResponseDTO
    {
        public StatusCode StatusCode { get; set; }
        public int TagId { get; set; }
    }
}
