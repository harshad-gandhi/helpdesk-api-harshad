using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IDirectMessageRepository
    {
        Task<DirectMessageSendResultDTO> SendDirectMessageAsync(DirectMessageSendRequestDTO directMessageSendRequestDTO);

        Task<List<DirectMessageGetResultDTO>> GetDirectMessagesBetweenUsersAsync(DirectMessagesGetRequestDTO directMessagesGetRequestDTO);

        Task<List<DirectMessageRecentResultDTO>> GetRecentDirectMessagesBetweenUsersAsync(int senderId);

        Task<DirectMessageResultDTO> MarkDirectMessageAsReadAsync(long messageId);

        Task<List<DirectMessageResultDTO>> MarkAllDirectMessageAsReadAsync(DirectMessageMarkAllAsReadRequestDTO directMessageMarkAllAsReadRequestDTO);

        Task<DirectMessageResultDTO> UpdateDirectMessageAsync(DirectMessageUpdateRequestDTO directMessageUpdateRequestDTO);

        Task<DirectMessageResultDTO> DeleteDirectMessageAsync(long messageId);

        Task<DirectMessageAttachmentResultDTO?> GetDirectMessageAttachmentAsync(int directMessageId);

        Task<DirectMessageResultDTO> UpdateDirectMessageAttachmentAsync(DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO);

        Task<DirectMessageSendResultDTO> SaveDirectMessageAttachmentAsync(DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO);

        Task<DirectMessageDeleteAttachmentResultDTO> DeleteDirectMessageAttachmentAsync(int directMessageId);

        Task<UsersResponseDTO?> GetUserByIdAsync(int userId);

        Task<List<UsersResponseDTO>> SearchUsersByKeyWordAsync(int userId, string keyword);

        Task<List<CountryDTO>> GetAllCountriesAsync();

        Task<IEnumerable<CountryDTO>> SearchCountriesAsync(string keyword);

        Task<CountryDTO?> GetCountryByCodeAsync(string countryCode);
    }
}