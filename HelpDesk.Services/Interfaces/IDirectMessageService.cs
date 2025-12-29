using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Interfaces
{
    public interface IDirectMessageService
    {
        Task<DirectMessageSendResponseDTO> SendDirectMessageAsync(DirectMessageSendRequestDTO directMessageSendRequestDTO);

        Task<List<DirectMessagesRecentMessagesResponseDTO>?> GetRecentDirectMessagesBetweenUsersAsync(int senderId);

        Task<DirectMessageMarkAsReadResponseDTO> MarkDirectMessageAsReadAsync(long messageId);

        Task<List<DirectMessageResultDTO>> MarkAllDirectMessageAsReadAsync(DirectMessageMarkAllAsReadRequestDTO directMessageMarkAllAsReadRequestDTO);

        Task<DirectMessageDeleteResponseDTO> DeleteDirectMessageAsync(long messageId);

        Task<DirectMessageUpdateResponseDTO> UpdateDirectMessageAsync(DirectMessageUpdateRequestDTO directMessageUpdateRequestDTO);

        Task<List<DirectMessageGetResponseDTO>?> GetDirectMessagesBetweenUsersAsync(DirectMessagesGetRequestDTO directMessagesGetRequestDTO);

        Task<DirectMessageUpdateFileAttachmentResponseDTO> UpdateDirectMessageAttachmentAsync(DirectMessageFileAttachmentUpdateRequestDTO directMessageFileAttachmentUpdateRequestDTO);

        Task<DirectMessageDeleteAttachmentResponseDTO> DeleteDirectMessageAttachmentAsync(int messageId);

        Task<UsersResponseDTO?> GetUserByIdAsync(int userId);

        Task<List<UsersResponseDTO>> SearchUsersByKeyWordAsync(int userId, string? keyword);

        Task<List<CountryDTO>> GetAllCountriesAsync();

        Task<IEnumerable<CountryDTO>> SearchCountriesAsync(string? keyword);
        
        Task<CountryDTO> GetCountryByCodeAsync(string countryCode);

    }
}