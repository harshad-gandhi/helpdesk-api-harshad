using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IChatMessagesRepository
    {
        Task<StoredProcedureResult<long>> CreateOrUpdateChatMessagesAsync(ChatMessagesDto chatMessagesDto);
        Task<StoredProcedureResult<bool>> DeleteChatMessagesAsync(long id);
        Task<ChatMessagesDto?> GetChatMessagesByIdAsync(int id);
        Task<List<ChatMessagesDto>> GetAllChatMessagesByFilterAsync(long id, string? searchTerm, bool isAgent);
    }   
}

