using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;
using Newtonsoft.Json;

namespace HelpDesk.Repositories.Implementations;

public class ArticleRepository(IDbConnectionFactory connectionFactory) : IArticleRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Adds a new article or updates an existing one in the knowledge base.  
    /// </summary>
    /// <param name="article"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:  
    /// ResultId: The newly created or updated article ID.  
    /// ReturnValue: The return status from the stored procedure.  
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> AddUpdateArticleAsync(ArticleCreateDto article)
    {
        const string spName = "usp_article_save";

        DynamicParameters parameters = new();

        parameters.Add("Id", article.Id);
        parameters.Add("ProjectId", article.ProjectId);
        parameters.Add("Title", article.Title);
        parameters.Add("Subtitle", article.Subtitle);
        parameters.Add("ArticleContent", article.Content);
        parameters.Add("Slug", article.Slug);
        parameters.Add("Status", (int)article.Status);
        parameters.Add("Visibility", (int)article.Visibility);
        parameters.Add("Language", (int)article.Language);
        parameters.Add("SortOrder", article.SortOrder);
        parameters.Add("MetaTitle", article.MetaTitle);
        parameters.Add("MetaDescription", article.MetaDescription);

        if (article.Id == null || article.Id == 0)
            parameters.Add("CreatedBy", article.CreatedBy);
        else
            parameters.Add("UpdatedBy", article.UpdatedBy);

        // Comma-separated CategoryIds
        string? categoryIds = article.CategoryIds != null && article.CategoryIds.Any()
            ? string.Join(",", article.CategoryIds)
            : null;
        parameters.Add("CategoryIds", categoryIds);

        // Comma-separated RelatedArticleIds
        string? relatedIds = article.RelatedArticleIds != null && article.RelatedArticleIds.Any()
            ? string.Join(",", article.RelatedArticleIds)
            : null;
        parameters.Add("RelatedArticleIds", relatedIds);

        // Output
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
    /// Retrieves an article by its ID.
    /// </summary>
    /// <param name="articleId"></param>
    /// <returns>
    /// An <see cref="ArticleDto"/> if found; otherwise, null.
    /// </returns>
    public async Task<ArticleDto?> GetArticleByIdAsync(int articleId)
    {
        const string spName = "usp_article_get_by_id";

        DynamicParameters parameters = new();
        parameters.Add("Id", articleId);

        ArticleDto? result = await _baseRepository.QueryFirstOrDefaultAsync<ArticleDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    /// <summary>
    /// Retrieves a list of articles based on the provided filter criteria.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>
    /// A collection of <see cref="ArticleDto"/> objects that match the filter criteria.
    /// </returns>
    public async Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter)
    {
        const string spName = "usp_article_get";

        DynamicParameters parameters = new();

        parameters.Add("ProjectId", filter.ProjectId);
        parameters.Add("Search", filter.Search);
        parameters.Add("Visibility", filter.Visibility);
        parameters.Add("Status", filter.Status);
        parameters.Add("AuthorName", filter.AuthorName);
        parameters.Add("CategoryName", filter.CategoryName);

        List<ArticleDto> articles = (await _baseRepository.QueryAsync<ArticleDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        )).AsList();
        return articles;
    }

    /// <summary>
    /// Deletes an article by its ID.
    /// </summary>
    /// <param name="articleId"></param>
    /// <param name="UpdatedBy"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the deleted article.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> DeleteArticleAsync(int articleId, int UpdatedBy)
    {
        const string spName = "usp_article_delete";

        DynamicParameters parameters = new();
        parameters.Add("Id", articleId);
        parameters.Add("UpdatedBy", UpdatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = articleId,
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }

    /// <summary>
    /// Archives an article by its ID.
    /// </summary>
    /// <param name="articleId"></param>
    /// <param name="updatedBy"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the archived article.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> ArchiveArticleAsync(int articleId, int updatedBy)
    {
        const string spName = "usp_article_update";

        DynamicParameters parameters = new();
        parameters.Add("Id", articleId);
        parameters.Add("UpdatedBy", updatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = articleId,
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
}
