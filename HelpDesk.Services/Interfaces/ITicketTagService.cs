using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface ITicketTagService
{
    Task AddTagToTicketAsync(TicketTagCreateRequestDto request);

    Task DeleteTicketTagAsync(TicketTagDeleteRequestDto request);
}
