using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IReportingRepository
    {
        Task<ReportingChatResultDTO> GetTotalConversationsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<CustomerSatisfactionRatingsDTO> GetCustomerSatisfactionRatingsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<ChatVolumeDTO>> GetChatVolumeAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<ReportingKBResultDTO> GetTotalKBDataAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<KBContentDistributionRatingDTO>> GetContentDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<KBTrendsDTO>> GetKBTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<ReportingTicketResultDTO> GetTotalTicketDataAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<TicketTrendsDTO>> GetTicketTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<TicketPriorityDistributionDTO>> GetPriorityDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO);
    }
}