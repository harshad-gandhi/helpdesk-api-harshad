using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Helpers;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides functionality to manage projects, including adding, updating, retrieving, 
/// deleting projects, managing chat widgets, and updating project statuses.
/// </summary>
public class ProjectService(IProjectRepository projectRepository, IStringLocalizer<Messages> localizer,
    IHttpContextAccessor httpContextAccessor, IImageService imageService) : IProjectService
{
    private readonly IImageService _imageService = imageService;
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Adds a new project with optional image upload.
    /// </summary>
    /// <param name="dto">The project creation request data.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> with operation result and status.</returns>
    /// <exception cref="DataAlreadyExistsException">Thrown when a project with the same name already exists.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<AddUpdateProjectResponseDTO> AddProjectAsync(AddProjectRequestDTO dto)
    {
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            string? savedPath = await _imageService.SaveImageFileAsync(dto.ImageFile, "uploads/projects");
            dto.ProjectImagePath = savedPath;
        }

        AddUpdateProjectResponseDTO result = await _projectRepository.AddProjectAsync(dto);
        StatusCode statusCode = (StatusCode)result.StatusCode;

        if (!Helper.IsSuccess(result.StatusCode))
        {
            throw statusCode switch
            {
                StatusCode.ProjectNameAlreadyExists => new DataAlreadyExistsException(_localizer["DATA_ALREADY_FOUND", _localizer["FIELD_PROJECT_NAME"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return result;
    }

    /// <summary>
    /// Updates an existing project, including optional image upload.
    /// </summary>
    /// <param name="dto">The project update request data.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> with operation result and status.</returns>
    /// <exception cref="DataAlreadyExistsException">Thrown when a project with the same name already exists.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<AddUpdateProjectResponseDTO> UpdateProjectAsync(UpdateProjectRequestDTO dto)
    {
        if (dto.ImageFile != null && dto.ImageFile.Length > 0)
        {
            string? savedPath = await _imageService.SaveImageFileAsync(dto.ImageFile, "uploads/projects");
            dto.ProjectImagePath = savedPath;
        }

        AddUpdateProjectResponseDTO result = await _projectRepository.UpdateProjectAsync(dto);
        StatusCode statusCode = (StatusCode)result.StatusCode;

        if (!Helper.IsSuccess(result.StatusCode))
        {
            throw statusCode switch
            {
                StatusCode.ProjectNameAlreadyExists => new DataAlreadyExistsException(_localizer["DATA_ALREADY_FOUND", _localizer["FIELD_PROJECT_NAME"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return result;
    }

    /// <summary>
    /// Saves chat widget settings for a project.
    /// </summary>
    /// <param name="dto">The chat widget update request data.</param>
    /// <returns>An <see cref="AddUpdateProjectResponseDTO"/> indicating operation status.</returns>
    /// <exception cref="InternalServerErrorException">Thrown when the operation fails unexpectedly.</exception>
    public async Task<AddUpdateProjectResponseDTO> SaveChatWidgetAsync(UpdateChatWidgetRequestDTO dto)
    {
        AddUpdateProjectResponseDTO result = await _projectRepository.UpdateChatWidgetAsync(dto);
        StatusCode statusCode = (StatusCode)result.StatusCode;

        if (!Helper.IsSuccess(result.StatusCode) && statusCode == StatusCode.InternalServerError)
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

        return result;
    }

    /// <summary>
    /// Retrieves all projects accessible to a specific user.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A list of <see cref="ProjectsListResultDTO"/> representing the user's projects.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<ProjectsListResultDTO>> GetAllProjectsAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        List<ProjectsListResultDTO>? projects = await _projectRepository.GetAllProjectsRawAsync(userId);

        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
        string? baseUrl = request != null ? $"{request.Scheme}://{request.Host}" : string.Empty;

        foreach (ProjectsListResultDTO? project in projects)
        {
            if (!string.IsNullOrEmpty(project.ProjectImage) && !string.IsNullOrEmpty(baseUrl))
                project.ProjectImage = $"{baseUrl}{project.ProjectImage}";
        }

        return projects;
    }

    /// <summary>
    /// Retrieves details of a project by its ID or code.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="projectCode">The unique code of the project.</param>
    /// <returns>A <see cref="ProjectResultDTO"/> containing project details.</returns>
    /// <exception cref="ValidationException">Thrown when the project ID is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the project does not exist.</exception>
    public async Task<ProjectResultDTO> GetProjectAsync(int? projectId, string? projectCode)
    {
        if (projectId <= 0)
            throw new ValidationException(_localizer["PROJECT_ID_REQUIRED"]);

        ProjectResultDTO result = await _projectRepository.GetProjectAsync(projectId, projectCode) ??
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_PROJECT"]]);

        // Append base URL if ProjectImage exists
        if (!string.IsNullOrEmpty(result.ProjectImage))
        {
            HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                string? baseUrl = $"{request.Scheme}://{request.Host}";
                result.ProjectImage = $"{baseUrl}{result.ProjectImage}";
            }
        }

        return result;
    }

    /// <summary>
    /// Retrieves chat widget settings for a specific project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>A <see cref="ChatWidgetSettingsDto"/> with the widget configuration.</returns>
    /// <exception cref="ValidationException">Thrown when the project ID is invalid.</exception>
    /// <exception cref="NotFoundException">Thrown when the project does not exist.</exception>
    public async Task<ChatWidgetSettingsDto> GetChatWidgetByProjectIdAsync(int projectId)
    {
        if (projectId <= 0)
            throw new ValidationException([_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_PROJECT_ID"]]]);

        ChatWidgetSettingsDto result = await _projectRepository.GetChatWidgetByProjectIdAsync(projectId) ??
        throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_PROJECT"]]);

        return result;
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to delete.</param>
    /// <param name="userIdStr">The ID of the user performing the deletion.</param>
    /// <exception cref="ValidationException">Thrown when the project ID is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the project does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the project has open tickets or active chat sessions.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task DeleteProjectAsync(int projectId, string userIdStr)
    {
        if (projectId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_PROJECT__ID"]]);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException(_localizer["INVALID_OR_MISSING_USER_ID"]);

        int result = await _projectRepository.DeleteProjectAsync(projectId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.ProjectNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_PROJECT"]]),
                StatusCode.OpenTickets => new BadRequestException(_localizer["PROJECT_HAS_OPEN_TICKETS"]),
                StatusCode.ActiveChatSession => new BadRequestException(_localizer["PROJECT_HAS_ACTIVE_CHAT"]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }
    }

    /// <summary>
    /// Retrieves projects accessible by a specific user for dropdown selection.
    /// </summary>
    /// <param name="userIdStr">The ID of the user as a string.</param>
    /// <returns>A list of <see cref="DropdownDTO"/> representing the user's projects.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    public async Task<List<DropdownDTO>> GetProjectsByUserIdAsync(string userIdStr)
    {
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

        return await _projectRepository.GetProjectsByUserIdAsync(userId);
    }

    /// <summary>
    /// Updates the enabled/disabled status of a project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="userIdStr">The ID of the user performing the update.</param>
    /// <returns>A message indicating whether the project was enabled or disabled successfully.</returns>
    /// <exception cref="ValidationException">Thrown when the project ID is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID is invalid or missing.</exception>
    /// <exception cref="NotFoundException">Thrown when the project does not exist.</exception>
    /// <exception cref="InternalServerErrorException">Thrown for unexpected server errors.</exception>
    public async Task<string> UpdateProjectEnabledStatusAsync(int projectId, string userIdStr)
    {
        if (projectId <= 0)
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_PROJECT__ID"]]);

        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedById))
            throw new UnauthorizedAccessException(_localizer["INVALID_OR_MISSING_USER_ID"]);

        int result = await _projectRepository.UpdateProjectEnabledStatusAsync(projectId, updatedById);
        StatusCode statusCode = (StatusCode)result;

        if (!Helper.IsSuccess(result))
        {
            throw statusCode switch
            {
                StatusCode.ProjectNotFound => new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_PROJECT"]]),
                _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
            };
        }

        return statusCode == StatusCode.ProjectEnabledSuccessfully ? _localizer["PROJECT_ENABLED_SUCCESSFULLY"] : _localizer["PROJECT_DISABLED_SUCCESSFULLY"];
    }

}
