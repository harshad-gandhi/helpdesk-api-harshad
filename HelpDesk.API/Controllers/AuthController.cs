using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Resources;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Constants;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles all authentication-related operations, including user registration, login, 
/// password reset, email verification, two-factor authentication (2FA), and token refresh.
/// </summary>
[ApiController]
[Route("api/auth")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class AuthController(IAuthService authService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Registers a new user with the provided registration details.
    /// Calls the authentication service to create the user and returns a response message.
    /// </summary>
    /// <param name="dto">The registration details including email, password, and other user information.</param>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
    {
        string? message = await _authService.RegisterAsync(dto);

        return _responseService.GetSuccessResponse(HttpStatusCode.Created, null, [message]);
    }

    /// <summary>
    /// Authenticates a user with the provided login credentials.
    /// Handles standard login and conditional two-factor authentication (2FA).
    /// Sets a secure refresh token cookie if applicable.
    /// </summary>
    /// <param name="dto">The login details including email/username and password.</param>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
    {
        LoginResponseDTO? result = await _authService.LoginAsync(dto);

        if (result.Requires2FA)
            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                result,
                [_localizer["TWO_FACTOR_REQUIRED_VERIFY_IDENTITY"]]
            );

        if (!string.IsNullOrWhiteSpace(result.RefreshToken) && result.RefreshTokenExpires.HasValue)
        {
            CookieOptions? cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpires
            };

                Response.Cookies.Append(SystemConstant.REFRESH_TOKEN_COOKIE_NAME, result.RefreshToken, cookieOptions);
            }

        var response = new
        {
            result.AccessToken,
            result.Requires2FA
        };

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, response, [_localizer["LOGIN_SUCCESSFULLY"]]);
    }

    /// <summary>
    /// Initiates a password reset request for the user with the given email.
    /// Sends a password reset link or code via email if the user exists.
    /// </summary>
    /// <param name="dto">The request containing the user's email.</param>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO dto)
    {
        string message = await _authService.ForgotPasswordAsync(dto.Email);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Resets the user's password using the provided token and new password.
    /// </summary>
    /// <param name="dto">The request containing the reset token and new password.</param>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO dto)
    {
        string? message = await _authService.ResetPasswordAsync(dto);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Verifies the user's email using the provided verification token.
    /// </summary>
    /// <param name="dto">The request containing the email verification token.</param>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationRequestDTO dto)
    {
        string? message = await _authService.VerifyEmailAsync(dto.Token);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Verifies a user's two-factor authentication (2FA) login code.
    /// Sets a secure refresh token cookie if available.
    /// </summary>
    /// <param name="dto">The request containing the 2FA login code.</param>
    [HttpPost("verify-2fa-login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Verify2FALogin([FromBody] TwoFALoginRequestDTO dto)
    {
        LoginResponseDTO? result = await _authService.Verify2FALoginAsync(dto);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken) && result.RefreshTokenExpires.HasValue)
        {
            CookieOptions? cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpires
            };

            Response.Cookies.Append(SystemConstant.REFRESH_TOKEN_COOKIE_NAME, result.RefreshToken, cookieOptions);
        }

        var response = new
        {
            result.Id,
            result.Email,
            result.Name,
            result.AvatarUrl,
            result.AccessToken
        };

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, response, [_localizer["LOGIN_SUCCESSFULLY"]]);
    }

    /// <summary>
    /// Logs in a user using a backup code generated during 2FA setup.
    /// Sets a secure refresh token cookie if available.
    /// </summary>
    /// <param name="dto">The request containing the backup code.</param>
    [HttpPost("login-with-backup-code")]
    public async Task<IActionResult> LoginWithBackupCode([FromBody] BackupCodeLoginRequestDTO dto)
    {
        LoginResponseDTO? result = await _authService.VerifyBackupCodeLoginAsync(dto);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken) && result.RefreshTokenExpires.HasValue)
        {
            CookieOptions? cookieOptions = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.RefreshTokenExpires
            };

            Response.Cookies.Append(SystemConstant.REFRESH_TOKEN_COOKIE_NAME, result.RefreshToken, cookieOptions);
        }

        var response = new
        {
            result.Id,
            result.Email,
            result.Name,
            result.AvatarUrl,
            result.AccessToken
        };

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, response, [_localizer["LOGIN_SUCCESSFULLY"]]);
    }

    /// <summary>
    /// Generates a new access token using a valid refresh token stored in the cookie.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException">Thrown if the refresh token cookie is not found.</exception>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue(SystemConstant.REFRESH_TOKEN_COOKIE_NAME, out string? refreshToken))
            throw new UnauthorizedAccessException(_localizer["CookieNotFound"]);

        string? newAccessToken = await _authService.GenerateNewAccessToken(refreshToken);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            new { accessToken = newAccessToken },
            [_localizer["GENERATE_NEW_ACCESS_TOKEN_SUCCESSFULLY"]]
        );
    }

    /// <summary>
    /// Logs out the current user by deleting the refresh token cookie.
    /// </summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        if (Request.Cookies.ContainsKey(SystemConstant.REFRESH_TOKEN_COOKIE_NAME))
        {
            Response.Cookies.Delete(SystemConstant.REFRESH_TOKEN_COOKIE_NAME, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
        }

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["LOGOUT_SUCCESSFULLY"]]);
    }
}