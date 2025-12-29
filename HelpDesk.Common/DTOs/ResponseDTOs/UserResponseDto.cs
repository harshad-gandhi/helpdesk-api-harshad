namespace HelpDesk.Common.DTOs.ResponseDTOs;

public class UserResponseDTO
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string UserPreferenceSettings { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ReportsToPersonName { get; set; } = string.Empty;
    public string ReportsToPersonEmail { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public bool IsTwoFactorAuthEnabled { get; set; }
    public bool IsPasskeyEnabled { get; set; }
}
