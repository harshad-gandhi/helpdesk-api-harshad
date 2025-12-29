using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface ITicketService
{
        Task<long> CreateTicketAsync(TicketCreateRequestDTO request);

        Task UpdateTicketAsync(TicketUpdateRequestDTO request);

        Task UpdateMultipleTicketsAsync(MultipleTicketUpdateDTO request);

        Task DeleteTicketAsync(TicketDeleteRequestDTO request);

        Task<TicketDetailsDto> GetTicketByIdAsync(long TicketId);

        Task<PagedTicketWithTagsDto> GetPaginatedTicketsAsync(GetTicketsByProjectRequestDto request);

        Task<List<DropdownDTO>> GetDepartmentsAsync();

        Task<List<DropdownDTO>> GetProjectsByUserIdAsync(int userId);

        Task<List<DepartmentUserListResponseDTO>> GetProjectMembersByProjectIdAsync(int projectId);

        Task<TicketEventWithAttachmentsDto> CreateTicketEventAsync(TicketEventCreateRequestDTO request);

        Task AddTagToTicketAsync(TicketTagCreateRequestDto request);

        Task DeleteTicketTagAsync(TicketTagDeleteRequestDto request);

        Task AddWatcherToTicketAsync(TicketWatcherCreateRequestDTO request);

        Task DeleteWatcherFromTicketAsync(TicketWatcherDeleteRequestDto request);
}
