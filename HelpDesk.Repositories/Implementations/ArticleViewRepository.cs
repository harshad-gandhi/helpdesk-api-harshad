using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class ArticleViewRepository(IDbConnectionFactory dbConnectionFactory) : IArticleViewRepository
{
    private readonly BaseRepository _baseRepository = new(dbConnectionFactory);

    /// <summary>
    /// Adds a view record for a specific article in the knowledge base.
    /// </summary>
    /// <param name="articleView"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the newly created view record.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> AddArticleView(ArticleViewDto articleView)
    {
        const string spName = "usp_article_view_save";

        DynamicParameters parameters = new();
        parameters.Add("ArticleId", articleView.ArticleId);
        parameters.Add("PersonId", articleView.PersonId);
        parameters.Add("IpAddress", articleView.IpAddress);
        parameters.Add("ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
        return new KnowledgeBaseResponseDto
        {
            ResultId = parameters.Get<int>("ResultId"),
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
}
