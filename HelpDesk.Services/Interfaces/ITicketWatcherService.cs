using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface ITicketWatcherService
{
    Task AddWatcherToTicketAsync(TicketWatcherCreateRequestDTO request);

    Task DeleteWatcherFromTicketAsync(TicketWatcherDeleteRequestDto request);
}
