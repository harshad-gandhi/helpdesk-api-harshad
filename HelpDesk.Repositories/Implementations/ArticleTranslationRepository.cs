using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class ArticleTranslationRepository(IDbConnectionFactory connectionFactory) : IArticleTranslationRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Adds a new article translation or updates an existing one in the knowledge base.
    /// </summary>
    /// <param name="translation"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The newly created or updated article translation ID.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> AddUpdateArticleTranslationAsync(ArticleTranslationDto translation)
    {
        const string spName = "usp_article_translation_save";

        DynamicParameters parameters = new();
        parameters.Add("Id", translation.Id);
        parameters.Add("ArticleId", translation.ArticleId);
        parameters.Add("Language", (int)translation.Language);
        parameters.Add("Title", translation.Title);
        parameters.Add("Subtitle", translation.Subtitle);
        parameters.Add("Content", translation.Content);
        parameters.Add("MetaTitle", translation.MetaTitle);
        parameters.Add("MetaDescription", translation.MetaDescription);
        parameters.Add("Slug", translation.Slug);
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
    /// Deletes an article translation from the knowledge base.
    /// </summary>
    /// <param name="translationId"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the deleted article translation.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> DeleteArticleTranslationAsync(int translationId)
    {
        const string spName = "usp_article_translation_delete";

        DynamicParameters parameters = new();
        parameters.Add("Id", translationId);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = translationId,
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }

}
