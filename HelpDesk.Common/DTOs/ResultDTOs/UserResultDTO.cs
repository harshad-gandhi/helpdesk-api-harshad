namespace HelpDesk.Common.DTOs.ResultDTOs;

public class UserResultDTO
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TotpSecretKey { get; set; } = string.Empty;
    public string UserPreferenceSettings { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string ReportsToPersonName { get; set; } = string.Empty;
    public string ReportsToPersonEmail { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsTwoFactorAuthEnabled { get; set; }
    public bool IsPasskeyEnabled { get; set; }
    public bool IsDeleted { get; set; }

    public DateTimeOffset? LastActivatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
