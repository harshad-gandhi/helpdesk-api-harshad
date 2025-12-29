using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IInvitationRepository
{
     Task<InvitationResultDTO?> GetInvitationDetailsByTokenAsync(Guid token);

     Task<int> InviteUserAsync(InviteProjectMemberRequestDTO dto);

     Task<int> DeleteInvitationAsync(int invitationId, int updatedById);

     Task<List<InvitationDTO>> GetAdminInvitationsAsync(int userId);

     Task<List<InvitationDTO>> GetAgentInvitationsAsync(int userId);

     Task<int> ResendInvitationAsync(int invitationId, int updatedById);
     
     Task<int> AcceptInvitationAsync(AcceptOrRejectInvitationRequestDTO dto);

     Task<int> RejectInvitationAsync(AcceptOrRejectInvitationRequestDTO dto);
}
