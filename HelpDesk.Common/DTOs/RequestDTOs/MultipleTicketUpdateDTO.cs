namespace HelpDesk.Common.DTOs.RequestDTOs;
public class MultipleTicketUpdateDTO
{
    public List<int> TicketIds { get; set; }
    public int? ParentTicketId { get; set; }
    public int UpdatedBy { get; set; }
    public int EventType { get; set; }
    public int PerformerType { get; set; }
    public int ActionType { get; set; }
    public string EventText { get; set; }
    public string? Metadata { get; set; }
    public bool IsInternal { get; set; }
}
