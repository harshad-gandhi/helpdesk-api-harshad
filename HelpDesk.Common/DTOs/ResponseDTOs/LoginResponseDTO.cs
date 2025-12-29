namespace HelpDesk.Common.DTOs.ResponseDTOs;

public class LoginResponseDTO
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool Requires2FA { get; set; }
    public bool RememberMe { get; set; }

    // Only populated when Requires2FA is false
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
}
