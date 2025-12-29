using System.Numerics;
using HelpDesk.Common.Enums;


namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class TicketCreateResponseDTO
    {
        public StatusCode StatusCode { get; set; }
        public long TicketId { get; set; }
    }
}