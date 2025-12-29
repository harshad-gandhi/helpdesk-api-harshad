using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles all admin-related operations, including retrieving admin details, managing invitations,
/// updating admin information, and deleting admins or invitations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/admins")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]

public class AdminController(IAdminService adminService, IResponseService<object> responseService,
    IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IAdminService _adminService = adminService;
    private readonly IResponseService<object> _responseService = responseService;

    /// <summary>
    /// Retrieves all admins filtered by active status.
    /// </summary>
    /// <param name="isActive">Optional flag to filter by active/inactive admins.</param>
    [HttpGet]
    public async Task<IActionResult> GetAdmins([FromQuery] bool? isActive)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<AdminsListResultDTO> admins = await _adminService.GetAdminsAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]), isActive);

        if (admins == null || admins.Count == 0)
        {
            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                admins,
                [_localizer["DATA_NOT_FOUND", _localizer["FIELD_ADMINS"]]]
            );
        }

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            admins
        );
    }

    /// <summary>
    /// Retrieves all invitations sends for admin role.
    /// </summary>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetAdminInvitations()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<InvitationDTO> invitations = await _adminService.GetAdminInvitationsAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        if (invitations == null || invitations.Count == 0)
        {
            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                null,
                [_localizer["DATA_NOT_FOUND", _localizer["FIELD_INVITATIONS"]]]
            );
        }

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            invitations
        );
    }

    /// <summary>
    /// Retrieves details of a specific admin by their user ID.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin to retrieve.</param>
    [HttpGet("{adminUserId}")]
    public async Task<IActionResult> GetAdminById(int adminUserId)
    {
        List<AdminResultDTO>? admin = await _adminService.GetAdminByIdAsync(adminUserId);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            admin
        );
    }

    /// <summary>
    /// Updates an existing admin’s details.
    /// </summary>
    /// <param name="dto">The update request including admin details and updatedBy info.</param>
    [HttpPost("update-admin")]
    public async Task<IActionResult> UpdateAdmin([FromBody] UpdateProjectMemberRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        dto.UpdatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        await _adminService.UpdateAdminAsync(dto);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_ADMIN"]]]
        );
    }

    /// <summary>
    /// Sends an invitation to a new user for admin role.
    /// </summary>
    /// <param name="request">Invitation request containing project and admin details.</param>
    [HttpPost("invite")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InviteAdmin([FromBody] InviteProjectMemberRequestDTO request)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.CreatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        await _adminService.InviteAdminAsync(request);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["INVITATION_SEND_SUCCESSFULLY"]]
        );
    }

    /// <summary>
    /// Resends a previously sent admin invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to resend.</param>
    [HttpPost("resend-invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendInvitation([FromQuery] int invitationId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _adminService.ResendInvitationAsync(invitationId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["RESEND_INVITATION_SUCCESS", _localizer["FIELD_INVITATION"]]]
        );
    }

    /// <summary>
    /// Deletes a specific admin.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin to delete.</param>
    [HttpDelete("{adminUserId}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAdmin(int adminUserId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _adminService.DeleteAdminAsync(adminUserId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_ADMIN"]]]
        );
    }

    /// <summary>
    /// Deletes a invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to delete.</param>
    [HttpDelete("invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteInvitation([FromQuery] int invitationId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _adminService.DeleteInvitationAsync(invitationId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_INVITATION"]]]
        );
    }

}
