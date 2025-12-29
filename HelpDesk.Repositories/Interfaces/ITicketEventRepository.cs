using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ITicketEventRepository
{
    Task<(int statusCode, IEnumerable<TicketEventWithAttachmentDto> eventDetails)> CreateTicketEventAsync(TicketEventCreateRequestDTO request);
}
