using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    public class ChatMessagesRepository(IDbConnectionFactory connectionFactory) : IChatMessagesRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateChatMessagesAsync
        public async Task<StoredProcedureResult<long>> CreateOrUpdateChatMessagesAsync(ChatMessagesDto chatMessagesDto)
        {
            const string spName = "usp_chat_messages_save";

            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@Id", chatMessagesDto.Id);
            parameters.Add("@ChatSessionId", chatMessagesDto.ChatSessionId);
            parameters.Add("@SenderId", chatMessagesDto.SenderId);
            parameters.Add("@SenderType", chatMessagesDto.SenderType);
            parameters.Add("@ChatMessage", chatMessagesDto.ChatMessage);
            parameters.Add("@MessageType", chatMessagesDto.MessageType);
            parameters.Add("@VisibleTo", chatMessagesDto.VisibleTo);
            parameters.Add("@IsDeleted", chatMessagesDto.IsDeleted);
            parameters.Add("@CreatedAt", chatMessagesDto.CreatedAt);
            parameters.Add("@UpdatedAt", chatMessagesDto.UpdatedAt);
            parameters.Add("@ReadAt", chatMessagesDto.ReadAt);
            parameters.Add("@ResultId", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(
               spName,
               parameters,
               commandType: CommandType.StoredProcedure
           );

            return new StoredProcedureResult<long>
            {
                Data = parameters.Get<long>("@ResultId"),
                ReturnValue = parameters.Get<int>("@ReturnValue"),
            };
        }
        #endregion

        #region DeleteChatMessagesAsync
        public async Task<StoredProcedureResult<bool>> DeleteChatMessagesAsync(long id)
        {
            const string spName = "usp_chat_messages_delete";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);

            int returnValue = parameters.Get<int>("@ReturnValue");

            return new StoredProcedureResult<bool>
            {
                Data = returnValue == (int)StatusCode.DeletedSuccessfully,
                ReturnValue = returnValue,
            };
        }

        #endregion

        #region  GetChatMessagesByIdAsync
        public async Task<ChatMessagesDto?> GetChatMessagesByIdAsync(int id)
        {
            const string spName = "usp_chat_messages_get_by_id";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id);

            ChatMessagesDto? chatMessage = await _baseRepository.QueryFirstOrDefaultAsync<ChatMessagesDto>(spName, parameters, commandType: CommandType.StoredProcedure);
            return chatMessage;

        }
        #endregion
        
         #region  GetAllChatMessagesByFilterAsync
        public async Task<List<ChatMessagesDto>> GetAllChatMessagesByFilterAsync(long id, string? searchTerm, bool isAgent)
        {
            const string spName = "usp_chat_messages_get_all_by_filter";

            var parameters = new DynamicParameters();
            
            parameters.Add("@ChatSessionId", id);
            parameters.Add("@SearchTerm", searchTerm);
            parameters.Add("@IsAgent", isAgent);
            
            List<ChatMessagesDto> chatMessages = (await _baseRepository.QueryAsync<ChatMessagesDto>(spName, parameters, commandType: CommandType.StoredProcedure)).AsList();
            return chatMessages;

        }
        #endregion
    
}

}

