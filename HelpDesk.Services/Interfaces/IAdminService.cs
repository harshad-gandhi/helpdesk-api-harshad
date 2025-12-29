using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IAdminService
{
    Task<List<AdminsListResultDTO>> GetAdminsAsync(string userIdStr, bool? isActive);
    Task<List<InvitationDTO>> GetAdminInvitationsAsync(string userIdStr);
    Task<List<AdminResultDTO>> GetAdminByIdAsync(int adminUserId);
    Task UpdateAdminAsync(UpdateProjectMemberRequestDTO dto);
    Task InviteAdminAsync(InviteProjectMemberRequestDTO dto);
    Task ResendInvitationAsync(int invitationId, string userIdStr);
    Task DeleteAdminAsync(int adminUserId, string userIdStr);
    Task DeleteInvitationAsync(int invitationId, string userIdStr);
}
