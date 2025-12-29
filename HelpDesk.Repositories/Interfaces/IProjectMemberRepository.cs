using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IProjectMemberRepository
{
    Task<List<AdminsListResultDTO>> GetAdminListAsync(int userId);

    Task<List<ProjectMembersListResultDTO>> GetAgentListAsync(int userId);

    Task<List<AdminResultDTO>> GetAdminByIdAsync(int adminUserId);

    Task<List<ProjectMemberResultDTO>> GetAgentByIdAsync(int agentUserId);

    Task<List<DropdownDTO>> GetReportsToListAsync(int roleId, int? agentUserId, int? departmentId);

    Task<int> DeleteAdminAsync(int adminUserId, int updatedBy);

    Task<int> DeleteAgentAsync(int agentUserId, int updatedBy);

    Task<int> UpdateProjectMemberAsync(UpdateProjectMemberRequestDTO dto);
}
