using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ITicketRepository
{
        Task<TicketCreateResponseDTO?> CreateTicketAsync(TicketCreateRequestDTO request);
        
        Task<int> UpdateTicketAsync(TicketUpdateRequestDTO request);

        Task<int> UpdateMultipleTicketsAsync(MultipleTicketUpdateDTO request);

        Task<int> DeleteTicketAsync(TicketDeleteRequestDTO request);

        Task<TicketDetailsFlatDto> GetTicketByIdAsync(long ticketId);

        Task<(IEnumerable<TicketDto> Tickets, IEnumerable<TicketTagDto> Tags, int TotalCount)> GetPaginatedTicketsAsync(GetTicketsByProjectRequestDto request);

        Task<List<UserListResponseDTO>> GetUserListAsync(int projectId);
}
