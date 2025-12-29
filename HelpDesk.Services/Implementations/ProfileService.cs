using OtpNet;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides functionality for managing user profiles, including retrieving profile information,
/// updating user details, managing passwords, enabling/disabling two-factor authentication (2FA),
/// and handling user preference settings.
/// </summary>
public class ProfileService(IImageService imageService, IUserRepository userRepository, IStringLocalizer<Messages> localizer,
    IHttpContextAccessor httpContextAccessor) : IProfileService
{
    private readonly IImageService _imageService = imageService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Retrieves the profile information of a user by their user ID.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A <see cref="UserResponseDTO"/> containing the user's profile data.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    public async Task<UserResponseDTO> GetProfileAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        UserResultDTO? user = await _userRepository.GetByConditionAsync(userId: userId)
            ?? throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]);

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
            user.AvatarUrl = request != null ? $"{request.Scheme}://{request.Host}/{user.AvatarUrl}" : string.Empty;
        }

        return new UserResponseDTO
        {
            Id = user.Id,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            UserPreferenceSettings = user.UserPreferenceSettings,
            IsTwoFactorAuthEnabled = user.IsTwoFactorAuthEnabled,
            IsPasskeyEnabled = user.IsPasskeyEnabled,
            Department = user.Department,
            ReportsToPersonName = user.ReportsToPersonName,
            ReportsToPersonEmail = user.ReportsToPersonEmail
        };
    }

    /// <summary>
    /// Enables two-factor authentication (2FA) for a user and returns the TOTP secret and OTP URI.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>An object containing the TOTP secret and OTP authentication URI.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="BadRequestException">Thrown when the operation fails due to invalid state.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<object> EnableTwoFactorAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        // Generate TOTP secret
        byte[]? secretBytes = KeyGeneration.GenerateRandomKey(20);
        string? base32Secret = Base32Encoding.ToString(secretBytes);

        EnableTwoFAResultDTO? result = await _userRepository.EnableTwofaAsync(userId, base32Secret)
            ?? throw new InternalServerErrorException("INTERNAL_SERVER");

        StatusCode statusCode = (StatusCode)result.StatusCode;

        if (!Helper.IsSuccess(result.StatusCode))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new BadRequestException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        // Generate OTP URI
        string? issuer = "HelpDesk";
        string? otpauthUri = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(result.Email)}?secret={base32Secret}&issuer={Uri.EscapeDataString(issuer)}&digits=6";

        return new
        {
            secret = base32Secret,
            otpauthUri
        };
    }

    /// <summary>
    /// Disables two-factor authentication (2FA) for a user.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A message indicating successful 2FA disablement.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> DisableTwoFactorAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        int result = await _userRepository.DisableTwofaAsync(userId);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["DISABLE_TWO_FACTOR_AUTH_SUCCESSFULLY"];
    }

    /// <summary>
    /// Verifies the 2FA setup code and generates backup codes for a user.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <param name="code">The verification code from the authenticator app.</param>
    /// <returns>A list of generated backup codes for the user.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user or secret key is missing.</exception>
    /// <exception cref="BadRequestException">Thrown when the verification code is invalid.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<List<string>> VerifyTwoFactorSetupAsync(string userIdStr, string code)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        UserResultDTO? user = await _userRepository.GetByConditionAsync(userId, null);

        if (user is null || string.IsNullOrEmpty(user.TotpSecretKey))
            throw new NotFoundException("USER_NOT_FOUND_OR_SECRET_KEY_MISSING");

        Totp? totp = new Totp(Base32Encoding.ToBytes(user.TotpSecretKey));
        bool isValid = totp.VerifyTotp(code, out long _, new VerificationWindow(1, 1));

        if (!isValid)
            throw new BadRequestException("INVALID_VERIFICATION_CODE");

        List<string>? backupCodes = BackupCodeHelper.GenerateBackupCodes();
        List<string>? hashedCodes = BackupCodeHelper.HashBackupCodes(backupCodes);

        string jsonHashedCodes = JsonSerializer.Serialize(hashedCodes);

        int result = await _userRepository.Update2FAAndStoreBackupCodesAsync(userId, jsonHashedCodes);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return backupCodes;
    }

    /// <summary>
    /// Updates the profile of a user, including optional profile image upload.
    /// </summary>
    /// <param name="dto">The user profile update request data.</param>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A message indicating successful profile update.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="DataAlreadyExistsException">Thrown when the email already exists.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> UpdateUserProfileAsync(UserProfileRequestDTO dto, string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            string? savedPath = await _imageService.SaveImageFileAsync(dto.ImageFile, "uploads/profile-pics");
            dto.ProfileImagePath = savedPath;
        }

        int result = await _userRepository.UpdateUserProfileAsync(dto, userId);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                StatusCode.EmailAlreadyExists => new DataAlreadyExistsException(_localizer["ENTITY_EXISTS", _localizer["Email"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["PROFILE_UPDATE_SUCCESSFULLY"];
    }

    /// <summary>
    /// Updates the password of a user after verifying the current password.
    /// </summary>
    /// <param name="dto">The change password request data.</param>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A message indicating successful password update.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the current password is incorrect or column name is invalid.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> UpdatePasswordAsync(ChangePasswordRequestDTO dto, string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        UserResultDTO? user = await _userRepository.GetByConditionAsync(userId: userId)
            ?? throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]);

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
        if (!passwordMatches)
            throw new BadRequestException(_localizer["CURRENT_PASSWORD_IS_INCORRECT"]);

        string? newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, BCrypt.Net.BCrypt.GenerateSalt(12));

        int result = await _userRepository.UpdateUserPartiallyAsync(userId, ColumnCodes.PasswordHash, newHashedPassword);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                StatusCode.INVALID_COLUMN_NAME => new BadRequestException(_localizer["INVALID_COLUMN_NAME"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["PASSWORD_UPDATE_SUCCESSFULLY"];
    }

    /// <summary>
    /// Updates the active/inactive status of a user.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <param name="IsActive">The desired active status.</param>
    /// <returns>A message indicating successful status update.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the column name is invalid.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> UpdateUserStatusAsync(string userIdStr, bool IsActive)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        int result = await _userRepository.UpdateUserPartiallyAsync(userId, ColumnCodes.IsActive, IsActive);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                StatusCode.INVALID_COLUMN_NAME => new BadRequestException(_localizer["INVALID_COLUMN_NAME"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["USER_STATUS_UPDATE_SUCCESSFULLY"];
    }

    /// <summary>
    /// Updates the user preference settings for a given user.
    /// </summary>
    /// <param name="dto">The user preference settings update request.</param>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A message indicating successful preference settings update.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> UpdateUserPreferenceSettingsAsync(UserPreferencesUpdateRequestDTO dto, string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        // Validate that at least one preference is being updated
        if (dto.Preferences == null || dto.Preferences.Count == 0)
            throw new BadHttpRequestException(_localizer["NO_PREFERENCES_PROVIDED"]);

        int result = await _userRepository.UpdateUserPreferenceSettingsAsync(dto, userId);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["USER_PREFERENCE_SETTING_UPDATE_SUCCESSFULLY"];
    }

}
