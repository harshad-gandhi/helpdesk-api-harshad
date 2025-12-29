using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces
{
    public interface IChatService
    {
        #region ChatSession
        Task<(ChatSessionDto, bool existingchat)> CreateChatSessionAsync(ChatSessionsCreateDto chatSessionCreateDto);
        Task<ChatSessionDto> UpdateChatSessionAsync(ChatSessionDto chatSessionDto);
        Task<bool> DeleteChatSessionAsync(long id);
        // Task<List<ChatSessionDto>> GetAllChatSessionsAsync();
        Task<ChatSessionsListDto> GetChatSessionByIdAsync(long Id);
        Task<PagedResult<ChatSessionsListDto>> GetAllChatSessionsByFilterAsync(ChatSessionsFilterDto chatSessionFilterDto);
        Task<List<ChatSessionsListDto>> GetRecentChatSessionsAsync(long projectId, int personId);
        Task<bool> ChatSessionMarkTrashAsync(int id, bool inTrash, DateTimeOffset updatedAt);
        Task<bool> ChatsessionMarkSpamAsync(int id, bool isSpam, DateTimeOffset updatedAt);
        Task<string> ChangeChatsessionStatusAsync(long chatsessionId, int newStatus);
        #endregion

        #region ChatTransfers
        Task<ChatTransfersDto> CreateChatTransfersAsync(ChatTransfersCreateDto chatTransferCreateDto);
        Task<ChatTransfersDto> UpdateChatTransfersAsync(ChatTransfersDto chatTransferDto);
        Task<bool> DeleteChatTransfersAsync(long id);
        Task<ChatTransfersDto> GetChatTransfersByIdAsync(int Id);
        Task<List<ChatTransfersDto>> GetAllChatTransfersByChatSessionIdAsync(long id);
        #endregion

        #region ChatsTagsMapping
        Task<ChatsTagsMappingDto> CreateChatsTagsMappingAsync(ChatsTagsMappingCreateDto chatsTagsMappingCreateDto);
        Task<ChatsTagsMappingDto> UpdateChatTagsMappingAsync(ChatsTagsMappingDto chatsTagsMappingDto);
        Task<bool> DeleteChatsTagsMappingAsync(long id);
        Task<ChatsTagsMappingDto> GetChatsTagsMappingByIdAsync(long id);
        #endregion

        #region ChatMessages
        Task<ChatMessagesDto> CreateChatMessagesAsync(ChatMessagesCreateDto chatMessagesCreateDto);
        Task<ChatMessagesDto> UpdateChatMessagesAsync(ChatMessagesDto chatMessagesDto);
        Task<bool> DeleteChatMessagesAsync(long id);
        Task<ChatMessagesDto> GetChatMessagesByIdAsync(int Id);
        Task<List<ChatMessagesDto>> GetAllChatMessagesByFilterAsync(long id, string? searchTerm, bool isAgent);
        #endregion
    }
}

