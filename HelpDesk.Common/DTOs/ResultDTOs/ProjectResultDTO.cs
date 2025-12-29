namespace HelpDesk.Common.DTOs.ResultDTOs;

public class ProjectResultDTO
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid ProjectCode { get; set; }
    public string? ProjectURL { get; set; }
    public string? Description { get; set; }
    public string? ProjectImage { get; set; }
    public bool EnableNewChatNotifications { get; set; }
    public bool EnableEmailNotifications { get; set; }
    public bool EnableSoundNotifications { get; set; }
    public string DirectChatLink { get; set; } = string.Empty;
    public string TicketForwardingEmail { get; set; } = string.Empty;
    public bool ProjectStatus { get; set; }
    public bool PreChatFormEnabled { get; set; }
}
