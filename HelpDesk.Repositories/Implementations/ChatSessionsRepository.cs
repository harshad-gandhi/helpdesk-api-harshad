using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    public class ChatSessionsRepository(IDbConnectionFactory connectionFactory) : IChatSessionsRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateChatSessionAsync
        public async Task<StoredProcedureResult<long>> CreateOrUpdateChatSessionAsync(ChatSessionDto chatSession)
        {
            const string spName = "usp_chat_session_save";

            DynamicParameters parameters = new();
            
            parameters.Add("@Id", chatSession.Id);
            parameters.Add("@ProjectId", chatSession.ProjectId);
            parameters.Add("@PersonId", chatSession.PersonId);
            parameters.Add("@UserId", chatSession.UserId);
            parameters.Add("@CountryId", chatSession.CountryId);
            parameters.Add("@ChatIpAddress", chatSession.ChatIpAddress);
            parameters.Add("@HostName", chatSession.HostName);
            parameters.Add("@OperatingSystem", chatSession.OperatingSystem);
            parameters.Add("@Browser", chatSession.Browser);
            parameters.Add("@ChatSessionStatus", chatSession.ChatSessionStatus);
            parameters.Add("@IsBotAnswered", chatSession.IsBotAnswered);
            parameters.Add("@IsSpam", chatSession.IsSpam);
            parameters.Add("@InTrash", chatSession.InTrash);
            parameters.Add("@IsDeleted", chatSession.IsDeleted);
            parameters.Add("@ChatRating", chatSession.ChatRating);
            parameters.Add("@FeedbackText", chatSession.FeedbackText);
            parameters.Add("@FeedbackSubmittedAt", chatSession.FeedBackSubmittedAt);
            parameters.Add("@IsHelpfulAgent", chatSession.HelpfullAgent);
            parameters.Add("@ResolutionStatus", chatSession.ResolutionStatus);
            parameters.Add("@CreatedAt", chatSession.CreatedAt);
            parameters.Add("@UpdatedAt", chatSession.UpdatedAt);
            parameters.Add("@EndedAt", chatSession.EndedAt);
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
                ReturnValue = parameters.Get<int>("@ReturnValue")
            };
        }
        #endregion

        #region DeleteChatSessionAsync
        public async Task<StoredProcedureResult<bool>> DeleteChatSessionAsync(long id)
        {
            const string spName = "usp_chat_session_delete";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);

            int returnValue = parameters.Get<int>("@ReturnValue");

            return new StoredProcedureResult<bool>
            {
                Data = returnValue == (int)StatusCode.DeletedSuccessfully,
                ReturnValue = returnValue
            };
        }

        #endregion

        #region  GetAllChatSessionsAsync
        public async Task<PagedResult<ChatSessionsListDto>> GetChatSessionsAsync(ChatSessionsFilterDto chatSessionFilterDto)
        {
            const string spName = "usp_chat_session_get_all_filter";

            DynamicParameters parameters = new();

            parameters.Add("@ProjectId", chatSessionFilterDto.ProjectId);
            parameters.Add("@PersonId", chatSessionFilterDto.PersonId);
            parameters.Add("@UserId", chatSessionFilterDto.UserId);
            parameters.Add("@FromDate", chatSessionFilterDto.FromDate);
            parameters.Add("@ToDate", chatSessionFilterDto.ToDate);
            parameters.Add("@SearchTerm", chatSessionFilterDto.SearchTerm);
            parameters.Add("@ChatSessionStatus", chatSessionFilterDto.ChatSessionStatus);
            parameters.Add("@HelpfulAgentStatus", chatSessionFilterDto.HelpfulAgentStatus);
            parameters.Add("@ResolutionStatus", chatSessionFilterDto.ResolutionStatus);
            parameters.Add("@IsSpam", chatSessionFilterDto.IsSpam);
            parameters.Add("@InTrash", chatSessionFilterDto.InTrash);
            parameters.Add("@PageNumber", chatSessionFilterDto.PageNumber);
            parameters.Add("@PageSize", chatSessionFilterDto.PageSize);
            parameters.Add("@SortBy", chatSessionFilterDto.SortBy);
            parameters.Add("@SortDirection", chatSessionFilterDto.SortDirection);
            parameters.Add("@IsAssignedToUserActive", chatSessionFilterDto.IsAssignedToUserActive);

             var result = await _baseRepository.QueryMultipleAsync(
                spName,
                parameters,
                async multi =>
                {
                    var chatSessions = multi.Read<ChatSessionsListDto>().AsList();

                    var totalCount = await multi.ReadSingleAsync<int>();

                    return new PagedResult<ChatSessionsListDto>
                    {
                        Items = chatSessions,
                        TotalCount = totalCount
                    };
                },
                commandType: CommandType.StoredProcedure);

            return result;

        }
        #endregion

         #region  GetRecentChatSessionsAsync
        public async Task<List<ChatSessionsListDto>> GetRecentChatSessionsAsync(long ProjectId, int PersonId)
        {
            const string spName = "usp_chat_history_get_by_person";

            DynamicParameters parameters = new();

            parameters.Add("@ProjectId", ProjectId);
            parameters.Add("@PersonId", PersonId);

             var result = await _baseRepository.QueryAsync<ChatSessionsListDto>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.AsList();

        }
        #endregion

        #region  GetChatSessionsByIdAsync
        public async Task<ChatSessionsListDto?> GetChatSessionsByIdAsync(long id)
        {
            const string spName = "usp_chat_session_get_by_id";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);

            ChatSessionsListDto? chatSession = await _baseRepository.QueryFirstOrDefaultAsync<ChatSessionsListDto>(spName, parameters, commandType: CommandType.StoredProcedure);
            return chatSession;

        }
        #endregion

    }

}

