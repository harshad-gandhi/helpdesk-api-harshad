using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IArticleTranslationService
{
     Task<ArticleTranslationDto> AddArticleTranslationAsync(ArticleTranslationCreateDto translation);
    Task<KnowledgeBaseResponseDto> DeleteArticleTranslationAsync(int translationId);
}
