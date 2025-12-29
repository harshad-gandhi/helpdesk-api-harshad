using Dapper;
using System.Data;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;
using Newtonsoft.Json;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing project members, including admins and agents.
/// </summary>
public class ProjectMemberRepository(IDbConnectionFactory connectionFactory) : IProjectMemberRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves a list of admins accessible by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <returns>A list of <see cref="AdminsListResultDTO"/> representing the admins.</returns>
    public async Task<List<AdminsListResultDTO>> GetAdminListAsync(int userId)
    {
        const string spName = "usp_admins_get";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<AdminsListResultDTO> result = await _baseRepository.QueryAsync<AdminsListResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves a list of agents accessible by the given user.
    /// </summary>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <returns>A list of <see cref="ProjectMembersListResultDTO"/> representing the agents.</returns>
    public async Task<List<ProjectMembersListResultDTO>> GetAgentListAsync(int userId)
    {
        const string spName = "usp_project_members_get";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<ProjectMembersListResultDTO> result = await _baseRepository.QueryAsync<ProjectMembersListResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves detailed information of an admin for editing purposes.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin user.</param>
    /// <returns>A list of <see cref="AdminResultDTO"/> containing the admin details.</returns>
    public async Task<List<AdminResultDTO>> GetAdminByIdAsync(int adminUserId)
    {
        const string spName = "usp_user_get_for_admin_edit";

        DynamicParameters? parameters = new();
        parameters.Add("@AdminUserId", adminUserId);

        IEnumerable<AdminResultDTO>? result = await _baseRepository.QueryAsync<AdminResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves detailed information of an agent for editing purposes.
    /// </summary>
    /// <param name="agentUserId">The ID of the agent user.</param>
    /// <returns>A list of <see cref="ProjectMemberResultDTO"/> containing the agent details.</returns>
    public async Task<List<ProjectMemberResultDTO>> GetAgentByIdAsync(int agentUserId)
    {
        const string spName = "usp_project_member_get_by_id";

        DynamicParameters? parameters = new();
        parameters.Add("@AgentUserId", agentUserId, DbType.Int32);

        IEnumerable<ProjectMemberResultDTO>? result = await _baseRepository.QueryAsync<ProjectMemberResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves a list of users that a member can report to based on role and department.
    /// </summary>
    /// <param name="roleId">The role ID of the member.</param>
    /// <param name="departmentId">The optional department ID.</param>
    /// <returns>A list of <see cref="DropdownDTO"/> representing the reporting options.</returns>
    public async Task<List<DropdownDTO>> GetReportsToListAsync(int roleId, int? agentUserId, int? departmentId)
    {
        const string spName = "usp_project_member_reports_to_get";

        DynamicParameters parameters = new();
        parameters.Add("@RoleId", roleId);
        parameters.Add("@AgentUserId", agentUserId);
        parameters.Add("@DepartmentId", departmentId);

        IEnumerable<DropdownDTO>? result = await _baseRepository.QueryAsync<DropdownDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Updates information for a project member, including role, status, and assigned projects.
    /// </summary>
    /// <param name="dto">The request DTO containing the updated member information.</param>
    /// <returns>An integer representing the result of the update operation.</returns>
    public async Task<int> UpdateProjectMemberAsync(UpdateProjectMemberRequestDTO dto)
    {
        const string spName = "usp_admin_or_agent_update";

        DynamicParameters? parameters = new();
        parameters.Add("@AgentUserId", dto.AgentUserId);
        parameters.Add("@AdminUserId", dto.AdminUserId);
        parameters.Add("@ChatLimit", dto.ChatLimit);
        parameters.Add("@RoleId", dto.Role);
        parameters.Add("@Status", dto.Status);
        parameters.Add("@DepartmentId", dto.Department);
        parameters.Add("@ReportsToId", dto.ReportsToPerson);
        parameters.Add("@UpdatedBy", dto.UpdatedBy);
        parameters.Add("@AgentProjectsJson", JsonConvert.SerializeObject(dto.AgentProjects));       // send AgentProjects as JSON
        parameters.Add("@AdminProjectsJson", JsonConvert.SerializeObject(dto.AdminProjects));       // send AdminProjects as JSON

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Deletes an admin user from the system.
    /// </summary>
    /// <param name="adminUserId">The ID of the admin user to delete.</param>
    /// <param name="updatedBy">The ID of the user performing the deletion.</param>
    /// <returns>An integer representing the result of the deletion operation.</returns>
    public async Task<int> DeleteAdminAsync(int adminUserId, int updatedBy)
    {
        const string spName = "usp_admin_delete";

        DynamicParameters? parameters = new();
        parameters.Add("@AdminUserId", adminUserId, DbType.Int32);
        parameters.Add("@UpdatedBy", updatedBy, DbType.Int32);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Deletes an agent user from the system.
    /// </summary>
    /// <param name="agentUserId">The ID of the agent user to delete.</param>
    /// <param name="updatedBy">The ID of the user performing the deletion.</param>
    /// <returns>An integer representing the result of the deletion operation.</returns>
    public async Task<int> DeleteAgentAsync(int agentUserId, int updatedBy)
    {
        const string spName = "usp_project_member_delete";

        DynamicParameters? parameters = new();
        parameters.Add("@AgentUserId", agentUserId, DbType.Int32);
        parameters.Add("@UpdatedBy", updatedBy, DbType.Int32);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

}
