using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces
{
    public interface IChatShortCutService
    {
        Task<List<ChatShortCutResponseDTO>> GetChatShortCutAsync(int projectId);

        Task<ChatShortCutResponseDTO> CreateChatShortCutAsync(ChatShortCutCreateRequestDTO chatShortCutCreateRequestDTO);

        Task<ChatShortCutResponseDTO> UpdateChatShortCutAsync(ChatShortCutUpdateRequestDTO chatShortCutUpdateRequestDTO);

        Task<ChatShortCutResponseDTO> DeleteChatShortCutAsync(int id, int userId);

        Task<ChatShortCutResponseDTO> ToggleChatShortCutVisibilityAsync(int id, int userId);

    }
}