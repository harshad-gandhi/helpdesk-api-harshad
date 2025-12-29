using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IArticleTranslationRepository
{
    Task<KnowledgeBaseResponseDto> AddUpdateArticleTranslationAsync(ArticleTranslationDto translation);
     Task<KnowledgeBaseResponseDto> DeleteArticleTranslationAsync(int translationId);
}
