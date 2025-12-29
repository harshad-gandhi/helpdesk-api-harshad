using System.Net;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles operations related to project invitations, including retrieving invitation details,
/// accepting an invitation, and rejecting an invitation.
/// </summary>
[ApiController]
[Route("api/invitation")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class InvitationController(IInvitationService invitationService, IStringLocalizer<Messages> localizer,
    IResponseService<object> responseService) : ControllerBase
{
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IInvitationService _invitationService = invitationService;
    private readonly IResponseService<object> _responseService = responseService;

    /// <summary>
    /// Retrieves the details of an invitation using its unique token.
    /// </summary>
    /// <param name="token">The unique GUID token for the invitation.</param>
    [HttpGet("{token}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvitationDetails(Guid token)
    {
        InvitationResultDTO? invitation = await _invitationService.GetInvitationDetailsByTokenAsync(token);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, invitation);
    }

    /// <summary>
    /// Accepts a invitation for admin/user role.
    /// </summary>
    /// <param name="request">The request containing the invitation token and user details.</param>
    [HttpPost("accept-invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptOrRejectInvitationRequestDTO request)
    {
        await _invitationService.AcceptInvitationAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["REDIRECT_TO_REGISTRATION"]]);
    }
    
    /// <summary>
    /// Rejects a invitation for admin/user role.
    /// </summary>
    /// <param name="request">The request containing the invitation token and user details.</param>
    [HttpPost("reject-invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RejectInvitation([FromBody] AcceptOrRejectInvitationRequestDTO request)
    {
        await _invitationService.RejectInvitationAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["INVITATION_REJECTED"]]);
    }
}
