using HelpDesk.Common.Enums;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IUserRepository
{
    Task<int> RegisterUserAsync(RegisterRequestDTO dto);

    Task<int> GenerateResetTokenAsync(string email);

    Task<int> ResetPasswordAsync(string token, string newPasswordHash);

    Task<int> VerifyEmailAsync(string token);

    Task<EnableTwoFAResultDTO?> EnableTwofaAsync(int userId, string secretKey);

    Task<UserResultDTO?> GetByConditionAsync(int? userId = null, string? email = null, string? refreshToken = null);

    Task<int> UpdateUserPartiallyAsync(int userId, ColumnCodes columnCode, object changedValue);

    Task<int> Update2FAAndStoreBackupCodesAsync(int userId, string jsonHashedCodes);

    Task<int> DisableTwofaAsync(int userId);

    Task<int> UpdateUserProfileAsync(UserProfileRequestDTO dto, int userId);

    Task<int> UpdateUserPreferenceSettingsAsync(UserPreferencesUpdateRequestDTO dto, int userId);

}
