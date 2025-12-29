using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class KBSearchRepository(IDbConnectionFactory dbConnectionFactory) : IKBSearchRepository
{
    private readonly BaseRepository _baseRepository = new(dbConnectionFactory);

    /// <summary>
    /// Saves search analytics data for the knowledge base.
    /// </summary>
    /// <param name="searchAnalytics"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the newly created search analytics record.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> SaveKbSearchAnalyticsAsync(KbSearchAnalyticsDto searchAnalytics)
    {
        const string spName = "usp_kb_search_analytics_save";

        DynamicParameters parameters = new();
        parameters.Add("ProjectId", searchAnalytics.ProjectId);
        parameters.Add("Keyword", searchAnalytics.Keyword);
        parameters.Add("ResultCount", searchAnalytics.ResultCount);
        parameters.Add("ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = parameters.Get<int>("ResultId"),
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
    
    /// <summary>
    /// Retrieves search analytics data for the knowledge base based on optional filters.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns>
    /// A collection of <see cref="KbSearchAnalyticsDto"/> representing the search analytics data.
    /// </returns>
    public async Task<IEnumerable<KbSearchAnalyticsDto>> GetKbSearchAnalyticsAsync(
        int? projectId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        const string spName = "usp_kb_search_analytics_get";

        DynamicParameters parameters = new();

        if (projectId.HasValue)
        {
            parameters.Add("ProjectId", projectId.Value);
        }
        if (fromDate != null)
        {
            parameters.Add("FromDate", fromDate?.ToDateTime(TimeOnly.MinValue));
        }
        if (toDate != null)
        {
            parameters.Add("ToDate", toDate?.ToDateTime(TimeOnly.MaxValue));
        }

        IEnumerable<KbSearchAnalyticsDto> result = await _baseRepository.QueryAsync<KbSearchAnalyticsDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
}
