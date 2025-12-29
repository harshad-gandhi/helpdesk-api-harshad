namespace HelpDesk.Common.DTOs.CommonDTOs;

public class ChatWidgetSettingsDto
{
    public int ChatWidgetId { get; set; }
    public string HeaderTitle { get; set; } = string.Empty;
    public string WelcomeMessage { get; set; } = string.Empty;
    public string ChatPosition { get; set; } = string.Empty;
    public string HeaderTextColor { get; set; } = string.Empty;
    public string HeaderBackground { get; set; } = string.Empty;
    public string AgentTextColor { get; set; } = string.Empty;
    public string AgentMessageBackground { get; set; } = string.Empty;
    public string CustomerTextColor { get; set; } = string.Empty;
    public string CustomerMessageBackground { get; set; } = string.Empty;
    public bool EnablePhoto { get; set; }
    public bool EnableAttachment { get; set; }
    public bool EnableEmoji { get; set; }
    public bool EnableEditOption { get; set; }
    public bool EnableDeleteOption { get; set; }
}
