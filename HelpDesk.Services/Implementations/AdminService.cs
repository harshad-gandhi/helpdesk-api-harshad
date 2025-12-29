using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Dapper;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Service implementation for managing admin users and their related operations.
/// </summary>
public class AdminService(IProjectMemberRepository projectMemberRepository, IInvitationRepository invitationRepository,
    IStringLocalizer<Messages> localizer, IHttpContextAccessor httpContextAccessor) : IAdminService
{
    private readonly IProjectMemberRepository _projectMemberRepository = projectMemberRepository;
    private readonly IInvitationRepository _invitationRepository = invitationRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Retrieves a list of admins accessible by a user, with optional filtering by active status.
    /// </summary>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <param name="isActive">Optional flag to filter by active status.</param>
    /// <returns>A list of <see cref="AdminsListResultDTO"/> representing admins.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<AdminsListResultDTO>> GetAdminsAsync(string userIdStr, bool? isActive)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        List<AdminsListResultDTO>? admins = await _projectMemberRepository.GetAdminListAsync(userId);

        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;

        if (request != null)
        {
            string? baseUrl = $"{request.Scheme}://{request.Host}";

            foreach (AdminsListResultDTO? admin in admins)
            {
                if (!string.IsNullOrEmpty(admin.AvatarUrl))
                    admin.AvatarUrl = $"{baseUrl}/{admin.AvatarUrl}";
            }
        }

        if (isActive.HasValue)
            admins = admins.Where(m => m.IsActive == isActive.Value).AsList();

        return admins;
    }

    /// <summary>
    /// Retrieves all invitations sent to admins for the given user.
    /// </summary>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <returns>A list of <see cref="InvitationDTO"/> representing admin invitations.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<InvitationDTO>> GetAdminInvitationsAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        return await _invitationRepository.GetAdminInvitationsAsync(userId);
    }

    /// <summary>
    /// Retrieves detailed information of an admin for editing purposes.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin user.</param>
    /// <returns>A list of <see cref="AdminResultDTO"/> containing admin details.</returns>
    /// <exception cref="ValidationException">Thrown when <paramref name="adminUserId"/> is less than or equal to zero.</exception>
    /// <exception cref="NotFoundException">Thrown when no admin is found.</exception>
    public async Task<List<AdminResultDTO>> GetAdminByIdAsync(int adminUserId)
    {
        if (adminUserId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_ADMIN_USER_ID"]]);

        List<AdminResultDTO>? admin = await _projectMemberRepository.GetAdminByIdAsync(adminUserId)
            ?? throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ADMIN"]]);

        return admin;
    }

    /// <summary>
    /// Updates information for an admin user.
    /// </summary>
    /// <param name="dto">The request DTO containing updated admin details.</param>
    /// <exception cref="NotFoundException">Thrown when the admin is not found.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task UpdateAdminAsync(UpdateProjectMemberRequestDTO dto)
    {
        int result = await _projectMemberRepository.UpdateProjectMemberAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.UserNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ADMIN"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Sends an invitation to a new admin.
    /// </summary>
    /// <param name="dto">The request DTO containing invitation details.</param>
    /// <exception cref="BadRequestException">Thrown when the invitation is invalid or conflicts with existing data.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task InviteAdminAsync(InviteProjectMemberRequestDTO dto)
    {
        int result = await _invitationRepository.InviteUserAsync(dto);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.AdminAlreadyExists => new BadRequestException(_localizer["ENTITY_EXISTS", _localizer["FIELD_ADMIN"]]),
                StatusCode.CannotAddSuperAdminAsAdmin => new BadRequestException(_localizer["NOT_INVITE_SUPER_ADMIN_AS_ADMIN"]),
                StatusCode.CannotAddAgentAsAdmin => new BadRequestException(_localizer["NOT_INVITE_AGENT_AS_ADMIN"]),
                StatusCode.InvitationAlreadySent => new BadRequestException(_localizer["INVITATION_ALREADY_SENT"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Resends an existing admin invitation.
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
    /// Deletes an admin from the system.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin user to delete.</param>
    /// <param name="userIdStr">The ID of the requesting user as a string.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the admin is not found.</exception>
    /// <exception cref="BadRequestException">Thrown when the admin has dependencies such as reports, tickets, or active chats.</exception>
    /// <exception cref="InternalServerErrorException">Thrown when an unexpected error occurs.</exception>
    public async Task DeleteAdminAsync(int adminUserId, string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        int result = await _projectMemberRepository.DeleteAdminAsync(adminUserId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.AdminNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ADMIN"]]),
                StatusCode.HasSomeoneReportsTo => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_REPORTS"]),
                StatusCode.OpenTickets => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_OPEN_TICKETS"]),
                StatusCode.ActiveChatSession => new BadRequestException(_localizer["PROJECT_MEMBER_HAS_ACTIVE_CHAT"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Deletes an admin invitation.
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
}
