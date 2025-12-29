namespace HelpDesk.Common.DTOs.ResultDTOs;

public class ProjectsListResultDTO
{
    public int ProjectId { get; set; }
    public string? ProjectImage { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
