using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IProfileService
{
    Task<object> EnableTwoFactorAsync(string userIdStr);
    Task<string> DisableTwoFactorAsync(string userIdStr);
    Task<List<string>> VerifyTwoFactorSetupAsync(string userIdStr, string code);
    Task<UserResponseDTO> GetProfileAsync(string userIdStr);
    Task<string> UpdateUserProfileAsync(UserProfileRequestDTO dto, string userIdStr);
    Task<string> UpdatePasswordAsync(ChangePasswordRequestDTO dto, string userIdStr);
    Task<string> UpdateUserStatusAsync(string userIdStr, bool IsActive);
    Task<string> UpdateUserPreferenceSettingsAsync(UserPreferencesUpdateRequestDTO dto, string userIdStr);
}
