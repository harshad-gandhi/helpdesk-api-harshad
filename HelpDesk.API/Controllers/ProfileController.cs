using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles operations related to the currently authenticated user's profile, including retrieving profile details,
/// updating profile information, managing two-factor authentication (2FA), changing passwords, updating user status,
/// and updating preference settings.
/// </summary>
[Authorize]
[ApiController]
[Route("api/profile")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProfileController(IProfileService profileService, IResponseService<object> responseService,
    IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;
    private readonly IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Retrieves the current user's profile details.
    /// </summary>
    [HttpGet("profile-details")]
    public async Task<IActionResult> GetProfile()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        UserResponseDTO? profile = await _profileService.GetProfileAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, profile);
    }

    /// <summary>
    /// Enables two-factor authentication (2FA) for the current user.
    /// Returns QR code and setup details for 2FA.
    /// </summary>
    [HttpPost("enable-2fa")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EnableTwoFactor()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        object? result = await _profileService.EnableTwoFactorAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));
        
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["SCAN_QR_TO_ENABLE_TWO_FACTOR"]]);
    }

    /// <summary>
    /// Disables two-factor authentication (2FA) for the current user.
    /// </summary>
    [HttpPost("disable-2fa")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DisableTwoFactor()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        string? message = await _profileService.DisableTwoFactorAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Verifies 2FA setup with a provided code and returns generated backup codes.
    /// </summary>
    [HttpPost("verify-2fa-setup")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Verify2FASetup([FromBody] string code)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<string>? backupCodes = await _profileService.VerifyTwoFactorSetupAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]), code);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            backupCodes,
            [_localizer["ENABLE_TWO_FACTOR_AUTH_SUCCESSFULLY"]]
        );
    }

    /// <summary>
    /// Updates the current user's profile information.
    /// </summary>
    [HttpPost("profile-update")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProfile([FromForm] UserProfileRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        string? message = await _profileService.UpdateUserProfileAsync(dto, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Updates the current user's password.
    /// </summary>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        string? message = await _profileService.UpdatePasswordAsync(dto, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Updates the current user's active status.
    /// </summary>
    [HttpPost("update-status")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        string? message = await _profileService.UpdateUserStatusAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]), dto.IsActive);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

    /// <summary>
    /// Updates the current user's preference settings like appearence, language.
    /// </summary>
    [HttpPost("update-preference-settings")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserPreferenceSettings([FromBody] UserPreferencesUpdateRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        string? message = await _profileService.UpdateUserPreferenceSettingsAsync(dto, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [message]);
    }

}
