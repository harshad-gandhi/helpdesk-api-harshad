using Dapper;
using System.Data;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing invitations and related operations.
/// </summary>
public class InvitationRepository(IDbConnectionFactory connectionFactory) : IInvitationRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves invitation details using a specific invitation token.
    /// </summary>
    /// <param name="token">The invitation token to look up.</param>
    /// <returns>Returns an <see cref="InvitationResultDTO"/> if found; otherwise, <c>null</c>.</returns>
    public async Task<InvitationResultDTO?> GetInvitationDetailsByTokenAsync(Guid token)
    {
        const string spName = "usp_invitation_get_details_by_invitation_token";

        DynamicParameters parameters = new();
        parameters.Add("@InvitationToken", token);

        InvitationResultDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<InvitationResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Invites a user to a project with a specific role and department.
    /// </summary>
    /// <param name="dto">The invitation details including email, role, department, and creator.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> InviteUserAsync(InviteProjectMemberRequestDTO dto)
    {
        const string spName = "usp_invitation_admin_or_agent";

        DynamicParameters parameters = new();
        parameters.Add("@Email", dto.Email.Trim().ToLower());
        parameters.Add("@RoleId", dto.RoleId);
        parameters.Add("@DepartmentId", dto.DepartmentId);
        parameters.Add("@ReportsToId", dto.ReportsToId);
        parameters.Add("@CreatedBy", dto.CreatedBy);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Deletes a project invitation.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to delete.</param>
    /// <param name="updatedById">The ID of the user performing the deletion.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> DeleteInvitationAsync(int invitationId, int updatedById)
    {
        const string spName = "usp_invitation_delete";

        DynamicParameters parameters = new();
        parameters.Add("@InvitationId", invitationId, DbType.Int32);
        parameters.Add("@UpdatedBy", updatedById, DbType.Int32);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Retrieves all project invitations associated with an admin user.
    /// </summary>
    /// <param name="userId">The ID of the admin user.</param>
    /// <returns>A list of <see cref="InvitationDTO"/> representing the admin's invitations.</returns>
    public async Task<List<InvitationDTO>> GetAdminInvitationsAsync(int userId)
    {
        const string spName = "usp_invitations_of_admins_get";

        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<InvitationDTO> result = await _baseRepository.QueryAsync<InvitationDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves all project invitations associated with an agent user.
    /// </summary>
    /// <param name="userId">The ID of the agent user.</param>
    /// <returns>A list of <see cref="InvitationDTO"/> representing the agent's invitations.</returns>
    public async Task<List<InvitationDTO>> GetAgentInvitationsAsync(int userId)
    {
        const string spName = "usp_invitations_of_agents_get";

        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<InvitationDTO> result = await _baseRepository.QueryAsync<InvitationDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Resends a project invitation to the specified user.
    /// </summary>
    /// <param name="invitationId">The ID of the invitation to resend.</param>
    /// <param name="updatedById">The ID of the user performing the resend operation.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> ResendInvitationAsync(int invitationId, int updatedById)
    {
        const string spName = "usp_invitation_resend";

        DynamicParameters parameters = new();
        parameters.Add("@InvitationId", invitationId);
        parameters.Add("@UpdatedById", updatedById);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Accepts a project invitation using the invitation token.
    /// </summary>
    /// <param name="dto">The request DTO containing the invitation token.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> AcceptInvitationAsync(AcceptOrRejectInvitationRequestDTO dto)
    {
        const string spName = "usp_invitation_validate";

        DynamicParameters? parameters = new();
        parameters.Add("@InvitationToken", Guid.Parse(dto.Token));

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Rejects a project invitation using the invitation token.
    /// </summary>
    /// <param name="dto">The request DTO containing the invitation token.</param>
    /// <returns>Returns an <see cref="int"/> representing the result of the operation.</returns>
    public async Task<int> RejectInvitationAsync(AcceptOrRejectInvitationRequestDTO dto)
    {
        const string spName = "usp_invitation_reject";

        DynamicParameters? parameters = new();
        parameters.Add("@InvitationToken", Guid.Parse(dto.Token));

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

}
