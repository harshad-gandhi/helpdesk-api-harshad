using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Resources;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.API.Controllers;

/// <summary>
/// Handles all project-related operations, including retrieving project details, managing projects,
/// updating project settings, managing chat widget settings, and deleting projects.
/// </summary>
[ApiController]
[Route("api/projects")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectController(IProjectService projectService, IResponseService<object> responseService,
    IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IProjectService _projectService = projectService;
    private readonly IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<ProjectsListResultDTO>? projects = await _projectService.GetAllProjectsAsync(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        if (projects == null || projects.Count == 0)
        {
            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                projects,
                [_localizer["DATA_NOT_FOUND", _localizer["FIELD_PROJECTS"]]]
            );
        }

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            projects
        );
    }

    /// <summary>
    /// Retrieves a project by its ID or project code.
    /// </summary>
    [HttpGet("project-by-id")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject([FromQuery] int? projectId, string? projectCode)
    {
        ProjectResultDTO project = await _projectService.GetProjectAsync(projectId, projectCode);
 
        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            project
        );
    }

    /// <summary>
    /// Retrieves chat widget settings for a specific project.
    /// </summary>
    [HttpGet("chat-widget/{projectId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChatWidgetByProjectId(int projectId)
    {
        ChatWidgetSettingsDto widgetSettings = await _projectService.GetChatWidgetByProjectIdAsync(projectId);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            widgetSettings
        );
    }

    /// <summary>
    /// Retrieves projects associated with the current user as dropdown options.
    /// </summary>
    [HttpGet("users-projects")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProjectsByUserId()
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        List<DropdownDTO> projects = await _projectService.GetProjectsByUserIdAsync(userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, projects);
    }

    /// <summary>
    /// Adds a new project.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddProject([FromForm] AddProjectRequestDTO request)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.CreatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        AddUpdateProjectResponseDTO result = await _projectService.AddProjectAsync(request);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            result,
            [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_PROJECT"]]]
        );
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    [HttpPost("update-project")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProject([FromForm] UpdateProjectRequestDTO request)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.UpdatedBy = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        AddUpdateProjectResponseDTO result = await _projectService.UpdateProjectAsync(request);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            result,
            [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_PROJECT"]]]
        );
    }

    /// <summary>
    /// Updates the enabled/disabled status of a project.
    /// </summary>
    [HttpPost("update-enabled-status")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProjectEnabledStatus([FromQuery] int projectId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        string message = await _projectService.UpdateProjectEnabledStatusAsync(projectId, userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [message]
        );
    }

    /// <summary>
    /// Saves or updates chat widget settings for a project.
    /// </summary>
    [HttpPost("save-chat-widget")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SaveChatWidget([FromBody] UpdateChatWidgetRequestDTO request)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        request.UserId = int.Parse(userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        AddUpdateProjectResponseDTO result = await _projectService.SaveChatWidgetAsync(request);

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_WIDGET_SETTING"]]]
        );
    }

    /// <summary>
    /// Deletes a project by its ID.
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProject([FromQuery] int projectId)
    {
        string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        await _projectService.DeleteProjectAsync(projectId, userIdStr
            ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            null,
            [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_PROJECT"]]]
        );
    }

}
