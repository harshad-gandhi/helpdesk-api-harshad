using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IChatsTagsMappingRepository
    {
        Task<StoredProcedureResult<int>> CreateOrUpdateChatTagsMappingAsync(ChatsTagsMappingDto chatsTagsMapping);
        Task<StoredProcedureResult<bool>> DeleteChatsTagsMappingAsync(long id);
        Task<ChatsTagsMappingDto?> GetChatsTagsMappingByIdAsync(long id);
    }

}

