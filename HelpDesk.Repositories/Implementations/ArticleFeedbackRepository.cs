using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class ArticleFeedbackRepository(IDbConnectionFactory dbConnectionFactory) : IArticleFeedbackRepository
{
    private readonly BaseRepository _baseRepository = new(dbConnectionFactory);

    /// <summary>
    /// Adds feedback for a specific article in the knowledge base.
    /// </summary>
    /// <param name="articleFeedback"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the newly created feedback entry.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> AddArticleFeedback(ArticleFeedbackDto articleFeedback)
    {
        const string spName = "usp_article_feedback_save";

        DynamicParameters parameters = new();
        parameters.Add("ArticleId", articleFeedback.ArticleId);
        parameters.Add("PersonId", articleFeedback.PersonId);
        parameters.Add("IsHelpful", articleFeedback.IsHelpful);
        parameters.Add("Comment", articleFeedback.Comment);
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
}
