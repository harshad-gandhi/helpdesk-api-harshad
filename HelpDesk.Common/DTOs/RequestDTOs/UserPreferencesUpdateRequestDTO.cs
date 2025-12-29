namespace HelpDesk.Common.DTOs.RequestDTOs;

public class UserPreferencesUpdateRequestDTO
{
    public Dictionary<string, string> Preferences { get; set; } = [];
}
