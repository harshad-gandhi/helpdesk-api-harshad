using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IProjectService
{
     Task<AddUpdateProjectResponseDTO> AddProjectAsync(AddProjectRequestDTO dto);

     Task<AddUpdateProjectResponseDTO> UpdateProjectAsync(UpdateProjectRequestDTO dto);

     Task<AddUpdateProjectResponseDTO> SaveChatWidgetAsync(UpdateChatWidgetRequestDTO dto);

     Task<List<ProjectsListResultDTO>> GetAllProjectsAsync(string userIdStr);

     Task<ProjectResultDTO> GetProjectAsync(int? projectId, string? projectCode);

     Task<ChatWidgetSettingsDto> GetChatWidgetByProjectIdAsync(int projectId);

     Task DeleteProjectAsync(int projectId, string userIdStr);

     Task<List<DropdownDTO>> GetProjectsByUserIdAsync(string userIdStr);

     Task<string> UpdateProjectEnabledStatusAsync(int projectId, string userIdStr);
}
