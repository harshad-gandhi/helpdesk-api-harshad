namespace HelpDesk.Common.DTOs.ResultDTOs;

public class ProjectMembersListResultDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Projects { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ReportsTo { get; set; } = string.Empty;   
    public string DepartmentName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? AvatarUrl { get; set; }
}
