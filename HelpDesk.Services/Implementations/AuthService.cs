using Microsoft.Extensions.Localization;
using OtpNet;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.Resources;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Services.Interfaces;
using System.Net.Http.Json;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides authentication-related services such as user registration, login, password reset, 
/// email verification, two-factor authentication (2FA), backup code login and token management.
/// </summary>
public class AuthService(IUserRepository userRepository, IUserBackupCodeRepository userBackupCodeRepository,
    ITokenService tokenService, IStringLocalizer<Messages> localizer, IHttpClientFactory httpClientFactory) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserBackupCodeRepository _userBackupCodeRepository = userBackupCodeRepository;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string TurnstileSecretKey = "0x4AAAAAAB5OK67KsoElAmSBO135vK7S2sA";

    /// <summary>
    /// Registers a new user by hashing their password and persisting their details.
    /// </summary>
    /// <param name="dto">The registration request containing user details.</param>
    /// <returns>A success message upon successful registration.</returns>
    /// <exception cref="DataAlreadyExistsException">Thrown when the email already exists.</exception>
    /// <exception cref="BadRequestException">Thrown when the invitation token is invalid or expired.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task<string> RegisterAsync(RegisterRequestDTO dto)
    {
        // Hash the password using BCrypt
        dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, BCrypt.Net.BCrypt.GenerateSalt(12));

        int result = await _userRepository.RegisterUserAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.EmailAlreadyExists => new DataAlreadyExistsException(_localizer["ENTITY_EXISTS", _localizer["Email"]]),
                StatusCode.InvitationTokenExpired => new BadRequestException(_localizer["INVITATION_TOKEN_EXPIRED"]),
                StatusCode.InvalidInvitationToken => new BadRequestException(_localizer["INVALID_INVITATION_TOKEN"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["REGISTRATION_SUCCESSFULLY"];
    }

    /// <summary>
    /// Authenticates a user by validating their credentials and handling 2FA if enabled.
    /// </summary>
    /// <param name="dto">The login request containing email, password, and remember-me flag.</param>
    /// <returns>A <see cref="LoginResponseDTO"/> containing tokens or requiring 2FA.</returns>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the email is not verified.</exception>
    /// <exception cref="BadRequestException">Thrown when the email or password is invalid.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception
    public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto)
    {
        // //Turnstile token verification
        //     if (string.IsNullOrWhiteSpace(dto.TurnstileToken))
        //         throw new BadRequestException(_localizer["CAPTCHA_REQUIRED"]);

        // var client = _httpClientFactory.CreateClient();
        // var content = new FormUrlEncodedContent(new Dictionary<string, string>
        // {
        //     { "secret", TurnstileSecretKey },
        //     { "response", dto.TurnstileToken }
        // });

        // var cloudflareResponse = await client.PostAsync("httTurnstileTokenps://challenges.cloudflare.com/turnstile/v0/siteverify", content);
        // var verifyResult = await cloudflareResponse.Content.ReadFromJsonAsync<TurnstileVerifyResponseDTO>();

        // if (verifyResult is null || !verifyResult.Success)
        //     throw new BadRequestException(_localizer["CAPTCHA_VERIFICATION_FAILED"]);

        //repo call
        UserResultDTO? user = await _userRepository.GetByConditionAsync(email: dto.Email);

        if (user is null)
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["User"]]);
        else if (!user.IsEmailVerified)
            throw new UnauthorizedAccessException(_localizer["EMAIL_NOT_VERIFIED_CHECK_LINK"]);

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!passwordMatches)
            throw new BadRequestException(_localizer["INVALID_EMAIL_OR_PASSWORD"]);

        if (user.IsTwoFactorAuthEnabled)
        {
            return new LoginResponseDTO
            {
                Id = user.Id,
                Requires2FA = true,
                RememberMe = dto.RememberMe
            };
        }

        string? accessToken = _tokenService.GenerateJwtToken(user.Email, user.Id);
        (string Token, DateTime Expires) refreshToken = _tokenService.GenerateRefreshToken(expiryDays: dto.RememberMe ? 7 : 1);

        int result = await _userRepository.UpdateUserPartiallyAsync(user.Id, ColumnCodes.RefreshToken, refreshToken.Token);
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

        return new LoginResponseDTO
        {
            Requires2FA = false,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpires = refreshToken.Expires
        };
    }

    /// <summary>
    /// Generates and sends a password reset token to the user’s email.
    /// </summary>
    /// <param name="email">The email address of the user requesting reset.</param>
    /// <returns>A success message when the reset email is sent.</returns>
    /// <exception cref="NotFoundException">Thrown when the email is not registered.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task<string> ForgotPasswordAsync(string email)
    {
        int result = await _userRepository.GenerateResetTokenAsync(email);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.EmailNotRegistered => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["Email"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["FORGOT_PASSWORD_EMAIL_SENT"];
    }

    /// <summary>
    /// Resets a user's password using a valid reset token.
    /// </summary>
    /// <param name="dto">The reset request containing reset token and new password.</param>
    /// <returns>A success message upon successful password reset.</returns>
    /// <exception cref="BadRequestException">Thrown when the token is invalid or expired.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task<string> ResetPasswordAsync(ResetPasswordRequestDTO dto)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, BCrypt.Net.BCrypt.GenerateSalt(12));

        int result = await _userRepository.ResetPasswordAsync(dto.ResetToken, passwordHash);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.InvalidPasswordResetToken => new BadRequestException(_localizer["INVALID_PASSWORD_RESET_TOKEN"]),
                StatusCode.PasswordResetTokenExpired => new BadRequestException(_localizer["PASSWORD_RESET_TOKEN_EXPIRED"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["PASSWORD_RESET_SUCCESSFULLY"];
    }

    /// <summary>
    /// Verifies a user’s email address using a verification token.
    /// </summary>
    /// <param name="token">The verification token.</param>
    /// <returns>A success message upon successful verification.</returns>
    /// <exception cref="BadRequestException">Thrown when the token is invalid or expired.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task<string> VerifyEmailAsync(string token)
    {
        int result = await _userRepository.VerifyEmailAsync(token);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.InvalidEmailVerificationToken => new BadRequestException(_localizer["INVALID_EMAIL_VERIFICATION_TOKEN"]),
                StatusCode.EmailVerificationTokenExpired => new BadRequestException(_localizer["EMAIL_VERIFICATION_TOKEN_EXPIRED"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return _localizer["EMAIL_VERIFIED_SUCCESSFULLY"];
    }

    /// <summary>
    /// Verifies a two-factor authentication (2FA) login attempt using a TOTP code.
    /// </summary>
    /// <param name="dto">The 2FA login request containing user ID, code, and remember-me flag.</param>
    /// <returns>A <see cref="LoginResponseDTO"/> with tokens on successful verification.</returns>
    /// <exception cref="NotFoundException">Thrown when the user or secret key is missing.</exception>
    /// <exception cref="BadRequestException">Thrown when the verification code is invalid.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task<LoginResponseDTO> Verify2FALoginAsync(TwoFALoginRequestDTO dto)
    {
        UserResultDTO? user = await _userRepository.GetByConditionAsync(userId: dto.UserId);
        if (user is null || string.IsNullOrEmpty(user.TotpSecretKey))
            throw new NotFoundException("USER_NOT_FOUND_OR_SECRET_KEY_MISSING");

        Totp? totp = new(Base32Encoding.ToBytes(user.TotpSecretKey));
        if (!totp.VerifyTotp(dto.Code, out _, new VerificationWindow(1, 1)))
            throw new BadRequestException("INVALID_VERIFICATION_CODE");

        string? accessToken = _tokenService.GenerateJwtToken(user.Email, user.Id);
        (string Token, DateTime Expires) refreshToken = _tokenService.GenerateRefreshToken(expiryDays: dto.RememberMe ? 7 : 1);

        int result = await _userRepository.UpdateUserPartiallyAsync(user.Id, ColumnCodes.RefreshToken, refreshToken.Token);
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

        return new LoginResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}",
            AvatarUrl = user.AvatarUrl,
            Requires2FA = false,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpires = refreshToken.Expires
        };
    }

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <returns>A newly generated access token.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the refresh token is invalid or not found.</exception>
    public async Task<string> GenerateNewAccessToken(string refreshToken)
    {
        UserResultDTO? user = await _userRepository.GetByConditionAsync(refreshToken: refreshToken)
            ?? throw new UnauthorizedAccessException(_localizer["DATA_NOT_FOUND", _localizer["REFRESH_TOKEN"]]);

        string? accessToken = _tokenService.GenerateJwtToken(user.Email, user.Id);

        return accessToken;
    }

    /// <summary>
    /// Verifies a login attempt using a 2FA backup code.
    /// </summary>
    /// <param name="dto">The backup code login request containing user ID, backup code, and remember-me flag.</param>
    /// <returns>A <see cref="LoginResponseDTO"/> with tokens on successful verification.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when backup codes are missing or invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the user is not found or missing a secret key.</exception>
    public async Task<LoginResponseDTO> VerifyBackupCodeLoginAsync(BackupCodeLoginRequestDTO dto)
    {
        List<BackupCodeResultDTO>? codes = await _userBackupCodeRepository.GetUnusedBackupCodesAsync(dto.UserId);

        if (codes is null || codes.Count == 0)
            throw new UnauthorizedAccessException(_localizer["DATA_NOT_FOUND", _localizer["USER_BACKUP_CODES"]]);

        BackupCodeResultDTO? matchedCode = codes.FirstOrDefault(c => BCrypt.Net.BCrypt.Verify(dto.BackupCode, c.BackupCodeHash))
            ?? throw new UnauthorizedAccessException("INVALID_BACKUP_CODE");

        await _userBackupCodeRepository.MarkBackupCodeAsUsedAsync(matchedCode.Id);

        UserResultDTO? user = await _userRepository.GetByConditionAsync(userId: dto.UserId);

        if (user is null || string.IsNullOrEmpty(user.TotpSecretKey))
            throw new NotFoundException("USER_NOT_FOUND_OR_SECRET_KEY_MISSING");

        string? accessToken = _tokenService.GenerateJwtToken(user.Email, user.Id);
        (string Token, DateTime Expires) refreshToken = _tokenService.GenerateRefreshToken(expiryDays: dto.RememberMe ? 7 : 1);

        return new LoginResponseDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}",
            AvatarUrl = user.AvatarUrl,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpires = refreshToken.Expires
        };
    }

}
