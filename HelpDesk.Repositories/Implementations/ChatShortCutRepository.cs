using System.Data;
using Dapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Repositories.Implementations
{
    public class ChatShortCutRepository(IDbConnectionFactory connectionFactory, IStringLocalizer<Messages> localizer) : IChatShortCutRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        /// <summary>
        /// Retrieves the list of chat shortcuts for a specific project by executing a stored procedure.
        /// </summary>
        /// <param name="projectId">The ID of the project for which to fetch chat shortcuts.</param>
        /// <returns>
        /// A <see cref="List{ChatShortCutResultDTO}"/> containing the chat shortcut data returned by the stored procedure.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<List<ChatShortCutResultDTO>> GetChatShortCutAsync(int projectId)
        {
            const string spName = "usp_chat_shortcuts_get";

            DynamicParameters parameters = new();

            parameters.Add("@ProjectId", projectId);

            List<ChatShortCutResultDTO>? dTOs = [.. await _baseRepository.QueryAsync<ChatShortCutResultDTO>(
                           spName,
                           parameters,
                           commandType: CommandType.StoredProcedure)];

            return dTOs;
        }

        /// <summary>
        /// Creates a new chat shortcut by executing a stored procedure.
        /// </summary>
        /// <param name="chatShortCutCreateRequestDTO">
        /// The request DTO containing the details of the chat shortcut to create, including ProjectId, UserId, ShortcutKey, ShortCutMessage, and IsPublic flag.
        /// </param>
        /// <returns>
        /// A <see cref="ChatShortCutResultDTO"/> containing the result of the create operation from the stored procedure.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<ChatShortCutResultDTO> CreateChatShortCutAsync(ChatShortCutCreateRequestDTO chatShortCutCreateRequestDTO)
        {
            const string spName = "usp_chat_shortcuts_save";

            DynamicParameters parameters = new();

            parameters.Add("@ProjectId", chatShortCutCreateRequestDTO.ProjectId);
            parameters.Add("@UserId", chatShortCutCreateRequestDTO.UserId);
            parameters.Add("@ShortcutKey", chatShortCutCreateRequestDTO.ShortCutKey);
            parameters.Add("@ShortCutMessage", chatShortCutCreateRequestDTO.ShortCutMessage);
            parameters.Add("@IsPublic", chatShortCutCreateRequestDTO.IsPublic);

            ChatShortCutResultDTO chatShortCutResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ChatShortCutResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return chatShortCutResultDTO;
        }

        /// <summary>
        /// Updates an existing chat shortcut by executing a stored procedure.
        /// </summary>
        /// <param name="chatShortCutUpdateRequestDTO">
        /// The request DTO containing the updated details of the chat shortcut, including Id, ProjectId, ShortcutKey, ShortCutMessage, and IsPublic flag.
        /// </param>
        /// <returns>
        /// A <see cref="ChatShortCutResultDTO"/> containing the result of the update operation from the stored procedure.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<ChatShortCutResultDTO> UpdateChatShortCutAsync(ChatShortCutUpdateRequestDTO chatShortCutUpdateRequestDTO)
        {
            const string spName = "usp_chat_shortcuts_save";

            DynamicParameters parameters = new();

            parameters.Add("@Id", chatShortCutUpdateRequestDTO.Id);
            parameters.Add("@UserId", chatShortCutUpdateRequestDTO.UserId);
            parameters.Add("@ProjectId", chatShortCutUpdateRequestDTO.ProjectId);
            parameters.Add("@ShortcutKey", chatShortCutUpdateRequestDTO.ShortCutKey);
            parameters.Add("@ShortCutMessage", chatShortCutUpdateRequestDTO.ShortCutMessage);
            parameters.Add("@IsPublic", chatShortCutUpdateRequestDTO.IsPublic);

            ChatShortCutResultDTO chatShortCutResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ChatShortCutResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return chatShortCutResultDTO;
        }

        /// <summary>
        /// Deletes a chat shortcut by executing a stored procedure.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResultDTO"/> containing the result of the delete operation from the stored procedure.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<ChatShortCutResultDTO> DeleteChatShortCutAsync(int id, int userId)
        {
            const string spName = "usp_chat_shortcuts_delete";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);
            parameters.Add("@UpdatedBy", userId);

            ChatShortCutResultDTO chatShortCutResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ChatShortCutResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return chatShortCutResultDTO;
        }

        /// <summary>
        /// Toggles the visibility (public/private) of a chat shortcut by executing a stored procedure.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut whose visibility is to be toggled.</param>
        /// <param name="userId">The ID of the user performing the visibility toggle.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResultDTO"/> containing the result of the visibility toggle operation from the stored procedure.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<ChatShortCutResultDTO> ToggleChatShortCutVisibilityAsync(int id, int userId)
        {
            const string spName = "usp_chat_shortcuts_visibility";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);
            parameters.Add("@UpdatedBy", userId);

            ChatShortCutResultDTO chatShortCutResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ChatShortCutResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return chatShortCutResultDTO;
        }
    }
}