using Dapper;
using System.Data;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;
using System.Text.Json;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for user-related database operations.
/// </summary>
public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves a user based on specified condition(s).
    /// </summary>
    /// <param name="userId">The unique identifier of the user (optional).</param>
    /// <param name="email">The email address of the user (optional).</param>
    /// <param name="refreshToken">The refresh token associated with the user (optional).</param>
    /// <returns>Returns a <see cref="UserResultDTO"/> if a matching user is found; otherwise, <c>null</c>.</returns>
    public async Task<UserResultDTO?> GetByConditionAsync(int? userId = null, string? email = null, string? refreshToken = null)
    {
        const string spName = "usp_user_get_by_condition";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId);
        parameters.Add("@Email", email?.Trim().ToLower());
        parameters.Add("@RefreshToken", refreshToken);

        UserResultDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<UserResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="dto">The registration details of the user.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> RegisterUserAsync(RegisterRequestDTO dto)
    {
        const string spName = "usp_user_registration";

        Guid? inviteGuid = Guid.TryParse(dto.InviteToken, out Guid parsedToken) ? parsedToken : null;

        DynamicParameters? parameters = new();
        parameters.Add("@FirstName", dto.FirstName.Trim());
        parameters.Add("@LastName", dto.LastName.Trim());
        parameters.Add("@Email", dto.Email.Trim().ToLower());
        parameters.Add("@PasswordHash", dto.Password);
        parameters.Add("@InviteToken", inviteGuid);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Generates a password reset token for a user.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> GenerateResetTokenAsync(string email)
    {
        const string spName = "usp_user_generate_password_reset_token";

        DynamicParameters? parameters = new();
        parameters.Add("@Email", email.Trim().ToLower());

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    /// <param name="token">The reset token issued to the user.</param>
    /// <param name="newPasswordHash">The new hashed password to be set.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> ResetPasswordAsync(string token, string newPasswordHash)
    {
        const string spName = "usp_user_reset_password";

        DynamicParameters? parameters = new();
        parameters.Add("@Token", token);
        parameters.Add("@NewPassword", newPasswordHash);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Verifies a user's email using a token.
    /// </summary>
    /// <param name="token">The email verification token.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> VerifyEmailAsync(string token)
    {
        const string spName = "usp_user_verify_email";

        DynamicParameters? parameters = new();
        parameters.Add("@Token", token);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Enables two-factor authentication (2FA) for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="secretKey">The secret key for 2FA setup.</param>
    /// <returns>Returns an <see cref="EnableTwoFAResultDTO"/> containing 2FA setup details.</returns>
    public async Task<EnableTwoFAResultDTO?> EnableTwofaAsync(int userId, string secretKey)
    {
        const string spName = "usp_user_enable_two_factor_authentication";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId);
        parameters.Add("@SecretKey", secretKey);

        EnableTwoFAResultDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<EnableTwoFAResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Updates a specific column of a user's record in the database.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="columnCode">The column code representing the field to be updated.</param>
    /// <param name="changedValue">The new value to be set.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> UpdateUserPartiallyAsync(int userId, ColumnCodes columnCode, object changedValue)
    {
        const string spName = "usp_user_partially_update";

        DynamicParameters? parameters = new();
        parameters.Add("UserId", userId, DbType.Int32);
        parameters.Add("ColumnCode", (int)columnCode, DbType.Int32);
        parameters.Add("ChangedValue", changedValue);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Stores backup codes and enables 2FA for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="jsonHashedCodes">The JSON string containing hashed backup codes.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> Update2FAAndStoreBackupCodesAsync(int userId, string jsonHashedCodes)
    {
        const string spName = "usp_user_enable_two_factor_auth_and_save_backup_codes";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId);
        parameters.Add("@JsonHashedCodes", jsonHashedCodes);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Disables two-factor authentication (2FA) for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> DisableTwofaAsync(int userId)
    {
        const string spName = "usp_user_disable_two_factor_auth";

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Updates a user's profile in the database.
    /// </summary>
    /// <param name="dto">The updated profile details of the user.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> UpdateUserProfileAsync(UserProfileRequestDTO dto, int userId)
    {
        const string spName = "usp_user_update_profile";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId);
        parameters.Add("@FirstName", dto.FirstName);
        parameters.Add("@LastName", dto.LastName);
        parameters.Add("@Email", dto.Email);
        parameters.Add("@PhoneNumber", dto.PhoneNumber);
        parameters.Add("@ImagePath", dto.ProfileImagePath);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Updates a user's preference settings in the database.
    /// </summary>
    /// <param name="dto">The updated preference settings of the user.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> UpdateUserPreferenceSettingsAsync(UserPreferencesUpdateRequestDTO dto, int userId)
    {
        const string spName = "usp_user_update_preference_settings";

        var preferencesJson = JsonSerializer.Serialize(dto.Preferences);

        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId);
        parameters.Add("@PreferencesJson", preferencesJson);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

}
