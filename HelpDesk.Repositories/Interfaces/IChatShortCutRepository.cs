using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IChatShortCutRepository
    {
        Task<List<ChatShortCutResultDTO>> GetChatShortCutAsync(int projectId);

        Task<ChatShortCutResultDTO> CreateChatShortCutAsync(ChatShortCutCreateRequestDTO chatShortCutCreateRequestDTO);

        Task<ChatShortCutResultDTO> UpdateChatShortCutAsync(ChatShortCutUpdateRequestDTO chatShortCutUpdateRequestDTO);

        Task<ChatShortCutResultDTO> DeleteChatShortCutAsync(int id, int userId);

        Task<ChatShortCutResultDTO> ToggleChatShortCutVisibilityAsync(int id, int userId);

    }
}