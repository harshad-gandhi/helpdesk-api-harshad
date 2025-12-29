using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces
{
  public interface IChatSessionsRepository
  {
    Task<StoredProcedureResult<long>> CreateOrUpdateChatSessionAsync(ChatSessionDto chatSession);
    Task<StoredProcedureResult<bool>> DeleteChatSessionAsync(long id);
    Task<PagedResult<ChatSessionsListDto>> GetChatSessionsAsync(ChatSessionsFilterDto chatSessionFilterDto);
    Task<List<ChatSessionsListDto>> GetRecentChatSessionsAsync(long ProjectId, int PersonId);
    Task<ChatSessionsListDto?> GetChatSessionsByIdAsync(long id);
  }


}
