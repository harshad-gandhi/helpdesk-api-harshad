namespace HelpDesk.Common.DTOs.RequestDTOs;

public class TicketWatcherDeleteRequestDto
{
    public int TicketId { get; set; }
    public int? ProjectMemberId { get; set; }   
    public int? DeletedBy { get; set; }      
    public int? EventType { get; set; }
    public int? PerformerType { get; set; }
    public string? EventText { get; set; }
    public string? Metadata { get; set; }
    public bool IsInternal { get; set; } = false;
}
