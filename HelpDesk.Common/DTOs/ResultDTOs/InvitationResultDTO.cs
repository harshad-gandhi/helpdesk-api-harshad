namespace HelpDesk.Common.DTOs.ResultDTOs;

public class InvitationResultDTO
{
    public string Role { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string ReportsTo { get; set; } = string.Empty;
}
