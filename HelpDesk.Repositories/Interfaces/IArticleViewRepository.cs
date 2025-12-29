using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IArticleViewRepository
{
     Task<KnowledgeBaseResponseDto> AddArticleView(ArticleViewDto articleView);
}
