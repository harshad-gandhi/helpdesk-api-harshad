using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequestDTO dto);
    Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto);
    Task<string> ForgotPasswordAsync(string email);
    Task<string> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<string> VerifyEmailAsync(string token);
    Task<LoginResponseDTO> Verify2FALoginAsync(TwoFALoginRequestDTO dto);
    Task<string> GenerateNewAccessToken(string refreshToken);
    Task<LoginResponseDTO> VerifyBackupCodeLoginAsync(BackupCodeLoginRequestDTO dto);
}
