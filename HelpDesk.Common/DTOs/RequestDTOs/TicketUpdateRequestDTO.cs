namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TicketUpdateRequestDTO
    {
        public long TicketId { get; set; }
        public int ColumnCode { get; set; }
        public object? ChangedValue { get; set; } 
        public int UpdatedBy { get; set; }
        public int EventType { get; set; }
        public int PerformerType { get; set; }
        public string EventText { get; set; } = string.Empty;
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; } = false;
    }
}