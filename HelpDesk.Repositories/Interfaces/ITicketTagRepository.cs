using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ITicketTagRepository
{
    Task<int> CreateTicketTagMappingAsync(TicketTagCreateRequestDto request);

    Task<int> DeleteTicketTagMappingAsync(TicketTagDeleteRequestDto request);
}
