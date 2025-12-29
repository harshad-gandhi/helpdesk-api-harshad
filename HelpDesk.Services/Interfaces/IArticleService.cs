using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IArticleService
{
    Task<ArticleCreateDto> AddArticleAsync(ArticleRequestDto article);
    Task<KnowledgeBaseResponseDto> DeleteArticleAsync(int articleId, int updatedBy);
    Task<ArticleDto> GetArticleByIdAsync(int articleId);
    // Task<IEnumerable<ArticleDto>> GetArticlesAsync(int? projectId = null, string search = null);
    Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter);
    Task<ArticleCreateDto> UpdateArticleAsync(ArticleUpdateDto article);
    Task<KnowledgeBaseResponseDto> ArchiveArticleAsync(int articleId, int updatedBy);

}
