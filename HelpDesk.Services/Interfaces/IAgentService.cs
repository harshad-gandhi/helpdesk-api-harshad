using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IAgentService
{
    Task<List<ProjectMembersListResultDTO>> GetAgentsAsync(string userIdStr, bool? isActive);
    Task<List<InvitationDTO>> GetAgentInvitationsAsync(string userIdStr);
    Task<List<ProjectMemberResultDTO>> GetAgentByIdAsync(int agentUserId);
    Task<List<DropdownDTO>> GetRolesAsync();
    Task<List<DropdownDTO>> GetDepartmentsAsync();
    Task<List<DropdownDTO>> GetReportsToListAsync(int roleId, int? agentUserId, int? departmentId);
    Task UpdateAgentAsync(UpdateProjectMemberRequestDTO dto);
    Task InviteAgentAsync(InviteProjectMemberRequestDTO dto);
    Task ResendInvitationAsync(int invitationId, string userIdStr);
    Task DeleteInvitationAsync(int invitationId, string userIdStr);
    Task DeleteAgentAsync(int agentUserId, string userIdStr);
}
