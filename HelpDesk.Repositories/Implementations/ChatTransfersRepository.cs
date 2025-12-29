using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    public class ChatTransfersRepository(IDbConnectionFactory connectionFactory) : IChatsTransfersRepository
    {
     private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateChatTransfersAsync
        public async Task<StoredProcedureResult<int>> CreateOrUpdateChatChatTransfersAsync(ChatTransfersDto chatTransferDto)
        {
            const string spName = "usp_chat_transfers_save";

            DynamicParameters parameters = new();

            parameters.Add("@Id", chatTransferDto.Id);
            parameters.Add("@ChatSessionId", chatTransferDto.ChatSessionId);
            parameters.Add("@FromUserId", chatTransferDto.FromUserId);
            parameters.Add("@ToUserId", chatTransferDto.ToUserId);
            parameters.Add("@TransferType", chatTransferDto.TransferType);
            parameters.Add("@Reason", chatTransferDto.Reason);
            parameters.Add("@@TransferredAt", chatTransferDto.TranferredAt);
            parameters.Add("@ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(
               spName,
               parameters,
               commandType: CommandType.StoredProcedure
           );

            return new StoredProcedureResult<int>
            {
                Data = parameters.Get<int>("@ResultId"),
                ReturnValue = parameters.Get<int>("@ReturnValue"),
            };
        }
        #endregion
        #region DeleteChatTransfersAsync
        public async Task<StoredProcedureResult<bool>> DeleteChatTransfersAsync(long id)
        {
            const string spName = "usp_chat_transfers_delete";

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

        #region  GetChatTransfersByIdAsync
        public async Task<ChatTransfersDto?> GetChatTransfersByIdAsync(int id)
        {
            const string spName = "usp_chat_transfers_get_by_id";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);

            ChatTransfersDto? chatTransfer = await _baseRepository.QueryFirstOrDefaultAsync<ChatTransfersDto>(spName, parameters, commandType: CommandType.StoredProcedure);
            return chatTransfer;

        }
        #endregion
        
        #region  GetAllChatTransfersByChatSessionIdAsync
        public async Task<List<ChatTransfersDto>> GetAllChatTransfersByChatSessionIdAsync(long id)
        {
            const string spName = "usp_chat_transfers_get_all_by_chat_session_id";

            DynamicParameters parameters = new();
            
            parameters.Add("@ChatSessionId", id);

            List<ChatTransfersDto> chatTransfers = (await _baseRepository.QueryAsync<ChatTransfersDto>(spName, parameters, commandType: CommandType.StoredProcedure)).AsList();
            return chatTransfers;

        }
        #endregion
}

}

