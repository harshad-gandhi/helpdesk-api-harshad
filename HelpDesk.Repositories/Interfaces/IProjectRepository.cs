using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<AddUpdateProjectResponseDTO> AddProjectAsync(AddProjectRequestDTO dto);

    Task<AddUpdateProjectResponseDTO> UpdateProjectAsync(UpdateProjectRequestDTO dto);

    Task<AddUpdateProjectResponseDTO> UpdateChatWidgetAsync(UpdateChatWidgetRequestDTO dto);

    Task<List<ProjectsListResultDTO>> GetAllProjectsRawAsync(int userId);

    Task<ProjectResultDTO?> GetProjectAsync(int? projectId, string? projectCode);

    Task<ChatWidgetSettingsDto?> GetChatWidgetByProjectIdAsync(int projectId);

    Task<int> DeleteProjectAsync(int projectId, int updatedBy);

    Task<List<DropdownDTO>> GetProjectsByUserIdAsync(int userId);

    Task<int> UpdateProjectEnabledStatusAsync(int projectId, int updatedBy);
}
