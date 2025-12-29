using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IArticleFeedbackRepository
{
     Task<KnowledgeBaseResponseDto> AddArticleFeedback(ArticleFeedbackDto articleFeedback);
   
}
