namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TicketTagCreateRequestDto
    {
        public int TicketId { get; set; }
        public int TagId { get; set;}
        public string TagName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public int EventType { get; set; }
        public int PerformerType { get; set; }
        public string EventText { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; } = false;
    }
}