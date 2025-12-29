using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using HelpDesk.Repositories.Interfaces;
using Dapper;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Service implementation for managing agent users and their related operations.
/// </summary>
public class AgentService(IRoleRepository roleRepository, IInvitationRepository invitationRepository,
    IProjectMemberRepository projectMemberRepository, IDepartmentRepository departmentRepository,
    IHttpContextAccessor httpContextAccessor, IStringLocalizer<Messages> localizer) : IAgentService
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IInvitationRepository _invitationRepository = invitationRepository;
    private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Retrieves a list of agents for the given user.
    /// </summary>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <param name="isActive">Optional flag to filter by active status.</param>
    /// <returns>A list of project members representing agents.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<ProjectMembersListResultDTO>> GetAgentsAsync(string userIdStr, bool? isActive)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        List<ProjectMembersListResultDTO>? members = await _projectMemberRepository.GetAgentListAsync(userId);

        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;

        if (request != null)
        {
            string? baseUrl = $"{request.Scheme}://{request.Host}";

            foreach (ProjectMembersListResultDTO? member in members)
            {
                if (!string.IsNullOrEmpty(member.AvatarUrl))
                    member.AvatarUrl = $"{baseUrl}{member.AvatarUrl}";
            }
        }

        if (isActive.HasValue)
            members = members.Where(m => m.IsActive == isActive.Value).AsList();

        return members;
    }

    /// <summary>
    /// Retrieves all agent invitations for the given user.
    /// </summary>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <returns>A list of project invitations.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<InvitationDTO>> GetAgentInvitationsAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        return await _invitationRepository.GetAgentInvitationsAsync(userId);
    }

    /// <summary>
    /// Retrieves details of an agent by their user ID.
    /// </summary>
    /// <param name="agentUserId">The user ID of the agent.</param>
    /// <returns>A list of project member details for the agent.</returns>
    /// <exception cref="ValidationException">Thrown when <paramref name="agentUserId"/> is less than or equal to zero.</exception>
    /// <exception cref="NotFoundException">Thrown when the agent data is not found.</exception>
    public async Task<List<ProjectMemberResultDTO>> GetAgentByIdAsync(int agentUserId)
    {
        if (agentUserId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_PROJECT_MEMBER_ID"]]);

        List<ProjectMemberResultDTO>? result = await _projectMemberRepository.GetAgentByIdAsync(agentUserId);

        if (result == null || result.Count == 0)
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["Agent"]]);

        return result;
    }

    /// <summary>
    /// Retrieves the list of available roles.
    /// </summary>
    /// <returns>A list of role dropdown items.</returns
    public async Task<List<DropdownDTO>> GetRolesAsync()
    {
        return await _roleRepository.GetRolesAsync();
    }

    /// <summary>
    /// Retrieves the list of available departments.
    /// </summary>
    /// <returns>A list of department dropdown items.</returns>
    public async Task<List<DropdownDTO>> GetDepartmentsAsync()
    {
        return await _departmentRepository.GetDepartmentsAsync();
    }

    /// <summary>
    /// Retrieves a list of "Reports To" options based on role and department.
    /// </summary>
    /// <param name="roleId">The role ID to filter by.</param>
    /// <param name="departmentId">Optional department ID to filter by.</param>
    /// <returns>A list of dropdown items representing supervisors.</returns>
    /// <exception cref="ValidationException">Thrown when <paramref name="roleId"/> is less than or equal to zero.</exception>
    public async Task<List<DropdownDTO>> GetReportsToListAsync(int roleId, int? agentUserId, int? departmentId)
    {
        if (roleId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_ROLE_ID"]]);

        return await _projectMemberRepository.GetReportsToListAsync(roleId, agentUserId, departmentId);
    }

    /// <summary>
    /// Updates details of an existing agent.
    /// </summary>
    /// <param name="dto">The update request DTO containing agent details.</param>
    /// <exception cref="NotFoundException">Thrown when the agent is not found.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task UpdateAgentAsync(UpdateProjectMemberRequestDTO dto)
    {
        int result = await _projectMemberRepository.UpdateProjectMemberAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_AGENT"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Invites a new agent to the project.
    /// </summary>
    /// <param name="dto">The invite request DTO containing agent information.</param>
    /// <exception cref="BadRequestException">
    /// Thrown when the agent already exists, 
    /// when inviting a Super Admin or Admin as an agent, 
    /// or when an invitation has already been sent.
    /// </exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task InviteAgentAsync(InviteProjectMemberRequestDTO dto)
    {
        int result = await _invitationRepository.InviteUserAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.AgentAlreadyExists => new BadRequestException(_localizer["ENTITY_EXISTS", _localizer["FIELD_AGENT"]]),
                StatusCode.CannotAddSuperAdminAsAgent => new BadRequestException(_localizer["NOT_INVITE_SUPER_ADMIN_AS_AGENT"]),
                StatusCode.CannotAddAdminAsAgent => new BadRequestException(_localizer["NOT_INVITE_ADMIN_AS_AGENT"]),
                StatusCode.InvitationAlreadySent => new BadRequestException(_localizer["INVITATION_ALREADY_SENT"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Resends an existing agent invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to resend.</param>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <exception cref="ValidationException">Thrown when <paramref name="invitationId"/> is less than or equal to zero.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="BadRequestException">Thrown when the invitation is not found.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task ResendInvitationAsync(int invitationId, string userIdStr)
    {
        if (invitationId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_INVITATION_ID"]]);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException(_localizer["INVALID_OR_MISSING_USER_ID"]);

        int result = await _invitationRepository.ResendInvitationAsync(invitationId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.InvitationNotFound => new BadRequestException(_localizer["DATA_NOT_FOUND", _localizer["INVITATION"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Deletes an existing agent invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to delete.</param>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <exception cref="ValidationException">Thrown when <paramref name="invitationId"/> is less than or equal to zero.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task DeleteInvitationAsync(int invitationId, string userIdStr)
    {
        if (invitationId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_INVITATION_ID"]]);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException(_localizer["INVALID_OR_MISSING_USER_ID"]);

        int result = await _invitationRepository.DeleteInvitationAsync(invitationId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result) && statusCode == StatusCode.InternalServerError)
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
    }

    /// <summary>
    /// Deletes an existing agent from the project.
    /// </summary>
    /// <param name="agentUserId">The ID of the agent to delete.</param>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the agent is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the agent has direct reports, open tickets, or active chat sessions.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task DeleteAgentAsync(int agentUserId, string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        int result = await _projectMemberRepository.DeleteAgentAsync(agentUserId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.AgentNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_AGENT"]]),
                StatusCode.HasSomeoneReportsTo => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_REPORTS"]),
                StatusCode.OpenTickets => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_OPEN_TICKETS"]),
                StatusCode.ActiveChatSession => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_ACTIVE_CHAT"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

}
