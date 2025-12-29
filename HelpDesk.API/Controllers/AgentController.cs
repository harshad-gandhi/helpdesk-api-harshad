using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles all agent-related operations, including retrieving agent details, managing invitations,
/// updating agent information, retrieving roles and departments, and deleting agents or invitations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/agents")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class AgentController(IAgentService agentService, IResponseService<object> responseService,
    IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IAgentService _agentService = agentService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IResponseService<object> _responseService = responseService;

    /// <summary>
    /// Retrieves all agents, optionally filtered by active status.
    /// </summary>
    /// <param name="isActive">Optional flag to filter active/inactive agents.</param>
    [HttpGet]
    public async Task<IActionResult> GetAgents([FromQuery] bool? isActive)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<ProjectMembersListResultDTO> agents = await _agentService.GetAgentsAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]), isActive);

        if (agents == null || agents.Count == 0)
        {
            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                agents,
                [_localizer["DATA_NOT_FOUND", _localizer["FIELD_AGENTS"]]]
            );
        }

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            agents
        );
    }

    /// <summary>
    /// Retrieves all invitations sends for user(agent) role.
    /// </summary>
    [HttpGet("invitations")]
    public async Task<IActionResult> GetAgentInvitations()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<InvitationDTO> invitations = await _agentService.GetAgentInvitationsAsync(userIdStr
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
    /// Retrieves details of a specific agent by their user ID.
    /// </summary>
    /// <param name="agentUserId">The ID of the agent to retrieve.</param>
    [HttpGet("{agentUserId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgentById(int agentUserId)
    {     
        List<ProjectMemberResultDTO>? member = await _agentService.GetAgentByIdAsync(agentUserId);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            member
        );
    }
    
    /// <summary>
    /// Retrieves all available roles for users.
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        List<DropdownDTO> roles = await _agentService.GetRolesAsync();

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, roles);
    }

    /// <summary>
    /// Retrieves all departments.
    /// </summary>
    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments()
    {
        List<DropdownDTO>? departments = await _agentService.GetDepartmentsAsync();

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            departments
        );
    }

    /// <summary>
    /// Retrieves a list of agents to whom a user can report based on role and optional department.
    /// </summary>
    /// <param name="roleId">The role ID for filtering.</param>
    /// <param name="departmentId">Optional department ID for filtering.</param>
    [HttpGet("reports-to")]
    public async Task<IActionResult> GetReportsToList([FromQuery] int roleId, [FromQuery] int? agentUserId, [FromQuery] int? departmentId)
    {
        List<DropdownDTO>? reportsTo = await _agentService.GetReportsToListAsync(roleId, agentUserId, departmentId);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            reportsTo
        );
    }

    /// <summary>
    /// Updates an existing agent’s details.
    /// </summary>
    /// <param name="dto">The update request including agent details and updatedBy info.</param>
    [HttpPost("update-agent")]
    public async Task<IActionResult> UpdateAgent([FromBody] UpdateProjectMemberRequestDTO dto)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        dto.UpdatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        await _agentService.UpdateAgentAsync(dto);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_AGENT"]]]
        );
    }

    /// <summary>
    /// Sends an invitation to a new user for agent role.
    /// </summary>
    /// <param name="request">Invitation request containing project and agent details.</param>
    [HttpPost("invite")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InviteAgent([FromBody] InviteProjectMemberRequestDTO request)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.CreatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        await _agentService.InviteAgentAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["INVITATION_SEND_SUCCESSFULLY"]]);
    }

    /// <summary>
    /// Resends a previously sent agent invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to resend.</param>
    [HttpPost("resend-invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendInvitation([FromQuery] int invitationId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _agentService.ResendInvitationAsync(invitationId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["RESEND_INVITATION_SUCCESS", _localizer["FIELD_INVITATION"]]]
        );
    }

    /// <summary>
    /// Deletes a specific agent.
    /// </summary>
    /// <param name="agentUserId">The ID of the agent to delete.</param>
    [HttpDelete("{agentUserId}")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAgent(int agentUserId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _agentService.DeleteAgentAsync(agentUserId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_AGENT"]]]
        );
    }
    
    /// <summary>
    /// Deletes a specific project invitation for agents.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to delete.</param>
    [HttpDelete("invitation")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteInvitation([FromQuery] int invitationId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _agentService.DeleteInvitationAsync(invitationId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_INVITATION"]]]
        );
    }

}
