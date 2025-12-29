using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IInvitationService
{
    Task AcceptInvitationAsync(AcceptOrRejectInvitationRequestDTO dto);
    Task RejectInvitationAsync(AcceptOrRejectInvitationRequestDTO dto);
    Task<InvitationResultDTO?> GetInvitationDetailsByTokenAsync(Guid token);
}
