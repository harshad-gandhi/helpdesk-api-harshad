using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IArticleRepository
{
    Task<KnowledgeBaseResponseDto> AddUpdateArticleAsync(ArticleCreateDto article);
    Task<ArticleDto?> GetArticleByIdAsync(int articleId);
    Task<KnowledgeBaseResponseDto> DeleteArticleAsync(int articleId, int UpdatedBy);
    Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter);
    Task<KnowledgeBaseResponseDto> ArchiveArticleAsync(int articleId, int updatedBy);
}
