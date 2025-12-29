using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IArticleFeddbackService
{
     Task<ArticleFeedbackDto> AddArticleFeedback(ArticleFeedbackCreateDto articleFeedback);
}
