using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IChatsTransfersRepository
    {
     Task<StoredProcedureResult<int>> CreateOrUpdateChatChatTransfersAsync(ChatTransfersDto chatTransferDto);
        Task<StoredProcedureResult<bool>> DeleteChatTransfersAsync(long id);
        Task<ChatTransfersDto?> GetChatTransfersByIdAsync(int id);
        Task<List<ChatTransfersDto>> GetAllChatTransfersByChatSessionIdAsync(long id);
}

}

