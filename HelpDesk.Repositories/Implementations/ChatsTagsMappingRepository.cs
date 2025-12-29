using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    public class ChatsTagsMappingRepository(IDbConnectionFactory connectionFactory) : IChatsTagsMappingRepository
    {
     private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateChatsTagsMappingAsync
        public async Task<StoredProcedureResult<int>> CreateOrUpdateChatTagsMappingAsync(ChatsTagsMappingDto chatsTagsMapping)
        {
            const string spName = "usp_chats_tags_mapping_save";

            DynamicParameters parameters = new();

            parameters.Add("@Id", chatsTagsMapping.Id);
            parameters.Add("@ChatSessionId", chatsTagsMapping.ChatSessionId);
            parameters.Add("@TagId", chatsTagsMapping.TagId);

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

        #region DeleteChatsTagsMappingAsync
        public async Task<StoredProcedureResult<bool>> DeleteChatsTagsMappingAsync(long id)
        {
            const string spName = "usp_chats_tags_mapping_delete";

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

        #region Get Chats tags Mapping By Id 
        public async Task<ChatsTagsMappingDto?> GetChatsTagsMappingByIdAsync(long id)
        {
            const string spName = "usp_chats_tags_mapping_get_by_id";

            DynamicParameters parameters = new();
            
            parameters.Add("@Id", id);

            ChatsTagsMappingDto? chatsTagsMapping = await _baseRepository.QueryFirstOrDefaultAsync<ChatsTagsMappingDto>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return chatsTagsMapping;
        }
        #endregion
}

}

