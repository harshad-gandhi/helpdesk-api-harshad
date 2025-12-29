using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IArticleViewService
{
     Task<ArticleViewDto> AddArticleView(ArticleViewCreateDto articleView);
}
