using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IKBSearchService
{
     Task<KbSearchAnalyticsDto> AddKbSearchAnalyticsAsync(KbSearchAnalyticsCreateDto searchAnalytics);
    Task<IEnumerable<KbSearchAnalyticsDto>> GetKbSearchAnalyticsAsync(int? projectId = null, DateOnly? fromDate = null, DateOnly? toDate = null);
}
