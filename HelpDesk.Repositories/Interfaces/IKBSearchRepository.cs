using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IKBSearchRepository
{
    Task<KnowledgeBaseResponseDto> SaveKbSearchAnalyticsAsync(KbSearchAnalyticsDto searchAnalytics);
    Task<IEnumerable<KbSearchAnalyticsDto>> GetKbSearchAnalyticsAsync(int? projectId = null, DateOnly? fromDate = null, DateOnly? toDate = null);

}
