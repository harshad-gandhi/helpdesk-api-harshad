namespace HelpDesk.Common.DTOs.ResultDTOs;

public class AdminResultDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int RoleId { get; set; }
    public int ChatLimit { get; set; }
    public int ProjectId { get; set; }
    public bool IsChatAgent { get; set; }
}
