namespace HelpDesk.Common.DTOs.CommonDTOs;

public class InvitationDTO
{
    public int InvitationId { get; set; }
    public int RoleId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public string ReportsTo { get; set; } = string.Empty;
    public string InvitedBy { get; set; } = string.Empty;
    public string InvitedByEmail { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string InvitedEmail { get; set; } = string.Empty;
}