using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ITicketWatcherRepository
{
    Task<int> CreateTicketWatcherAsync(TicketWatcherCreateRequestDTO request);

    Task<int> DeleteTicketWatcherAsync(TicketWatcherDeleteRequestDto request);
}
