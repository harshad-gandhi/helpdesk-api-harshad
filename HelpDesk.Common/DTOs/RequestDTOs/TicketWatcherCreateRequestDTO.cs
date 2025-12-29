namespace HelpDesk.Common.DTOs.RequestDTOs;

public class TicketWatcherCreateRequestDTO
{
    public int TicketId { get; set; }
    public int ProjectMemberId { get; set; }
    public int CreatedBy { get; set; }
    public int EventType { get; set; }
    public int PerformerType { get; set; }
    public string EventText { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public bool IsInternal { get; set; } = false;
}

