using Dapper;
using System.Data;
using Newtonsoft.Json;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing project-related operations.
/// </summary>
public class ProjectRepository(IDbConnectionFactory connectionFactory) : IProjectRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Adds a new project to the database.
    /// </summary>
    /// <param name="dto">The request DTO containing project details.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> containing the created project's ID and status code.</returns>
    public async Task<AddUpdateProjectResponseDTO> AddProjectAsync(AddProjectRequestDTO dto)
    {
        const string spName = "usp_project_save";

        DynamicParameters parameters = new();
        parameters.Add("@Name", dto.Name);
        parameters.Add("@LiveProjectUrl", dto.LiveProjectUrl);
        parameters.Add("@Description", dto.Description);
        parameters.Add("@ProjectImage", dto.ProjectImagePath);
        parameters.Add("@ProjectSetting", dbType: DbType.String, value: null);
        parameters.Add("@CreatedBy", dto.CreatedBy);
        parameters.Add("@UpdatedBy", dto.CreatedBy);
        parameters.Add("@ProjectId", DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _baseRepository.ExecuteAsync(
           spName,
           parameters,
           commandType: CommandType.StoredProcedure
       );

        return new AddUpdateProjectResponseDTO
        {
            ProjectId = parameters.Get<int?>("@ProjectId"),
            StatusCode = parameters.Get<int>("@StatusCode")
        };
    }

    /// <summary>
    /// Updates an existing project in the database.
    /// </summary>
    /// <param name="dto">The request DTO containing updated project details.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> containing the project's ID and status code.</returns>
    public async Task<AddUpdateProjectResponseDTO> UpdateProjectAsync(UpdateProjectRequestDTO dto)
    {
        const string spName = "usp_project_save";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", dto.Id, DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@Name", dto.Name);
        parameters.Add("@LiveProjectUrl", dto.LiveProjectUrl);
        parameters.Add("@Description", dto.Description);
        parameters.Add("@ProjectImage", dto.ProjectImagePath);

        parameters.Add("@ProjectSetting",
            dto.Settings is string strSettings
                ? strSettings
                : dto.Settings != null ? JsonConvert.SerializeObject(dto.Settings) : null,
            DbType.String
        );

        parameters.Add("@CreatedBy", null, DbType.Int32);
        parameters.Add("@UpdatedBy", dto.UpdatedBy);
        parameters.Add("@IsProjectEnable", dto.IsProjectEnable, DbType.Boolean);
        parameters.Add("@IsPrechatFormEnable", dto.IsPrechatFormEnable, DbType.Boolean);
        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _baseRepository.ExecuteAsync(
           spName,
           parameters,
           commandType: CommandType.StoredProcedure
       );

        return new AddUpdateProjectResponseDTO
        {
            ProjectId = parameters.Get<int?>("@ProjectId"),
            StatusCode = parameters.Get<int>("@StatusCode")
        };

    }

    /// <summary>
    /// Updates the chat widget settings for a project.
    /// </summary>
    /// <param name="dto">The request DTO containing chat widget settings.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> containing the project's ID and status code.</returns>
    public async Task<AddUpdateProjectResponseDTO> UpdateChatWidgetAsync(UpdateChatWidgetRequestDTO dto)
    {
        const string spName = "usp_chat_widget_save";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", dto.ProjectId, DbType.Int32);
        parameters.Add("@UserId", dto.UserId, DbType.Int32);

        parameters.Add("@WidgetSetting",
            dto.WidgetSetting is string strSetting
                ? strSetting
                : dto.WidgetSetting != null ? JsonConvert.SerializeObject(dto.WidgetSetting) : null,
            DbType.String
        );

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new AddUpdateProjectResponseDTO
        {
            ProjectId = parameters.Get<int?>("@ProjectId"),
            StatusCode = parameters.Get<int>("@StatusCode")
        };
    }

    /// <summary>
    /// Retrieves all projects for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="ProjectsListResultDTO"/> representing the projects.</returns>
    public async Task<List<ProjectsListResultDTO>> GetAllProjectsRawAsync(int userId)
    {
        const string spName = "usp_projects_get";

        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<ProjectsListResultDTO>? result = await _baseRepository.QueryAsync<ProjectsListResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves a specific project by ID or project code.
    /// </summary>
    /// <param name="projectId">The ID of the project (optional).</param>
    /// <param name="projectCode">The project code (optional).</param>
    /// <returns>A <see cref="ProjectResultDTO"/> if found; otherwise, null.</returns>
    public async Task<ProjectResultDTO?> GetProjectAsync(int? projectId, string? projectCode)
    {
        const string spName = "usp_project_get_by_id";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", projectId);
        parameters.Add("@ProjectCode", projectCode);

        ProjectResultDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<ProjectResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Retrieves chat widget settings for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A <see cref="ChatWidgetSettingsDto"/> if found; otherwise, null.</returns>
    public async Task<ChatWidgetSettingsDto?> GetChatWidgetByProjectIdAsync(int projectId)
    {
        const string spName = "usp_chatwidget_get_by_project_id";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", projectId, DbType.Int32);

        ChatWidgetSettingsDto? result = await _baseRepository.QueryFirstOrDefaultAsync<ChatWidgetSettingsDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Deletes a project from the database.
    /// </summary>
    /// <param name="projectId">The ID of the project to delete.</param>
    /// <param name="updatedBy">The ID of the user performing the deletion.</param>
    /// <returns>An integer representing the result of the operation.</returns>
    public async Task<int> DeleteProjectAsync(int projectId, int updatedBy)
    {
        const string spName = "usp_project_delete";

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", projectId, DbType.Int32);
        parameters.Add("@UpdatedBy", updatedBy, DbType.Int32);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Retrieves projects associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of <see cref="DropdownDTO"/> representing the user's projects.</returns>
    public async Task<List<DropdownDTO>> GetProjectsByUserIdAsync(int userId)
    {
        const string spName = "usp_projects_get_by_user";

        DynamicParameters? parameters = new();
        parameters.Add("@UserId", userId, DbType.Int32);

        IEnumerable<DropdownDTO>? result = await _baseRepository.QueryAsync<DropdownDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Toggles the active/enabled status of a project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="updatedBy">The ID of the user performing the update.</param>
    /// <returns>An integer representing the result of the operation.</returns>
    public async Task<int> UpdateProjectEnabledStatusAsync(int projectId, int updatedBy)
    {
        const string spName = "usp_project_toggle_active_status";

        DynamicParameters? parameters = new();
        parameters.Add("@ProjectId", projectId);
        parameters.Add("@UpdatedBy", updatedBy);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

}
