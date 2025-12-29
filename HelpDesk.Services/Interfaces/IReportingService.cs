using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Services.Interfaces
{
    public interface IReportingService
    {
        Task<ReportingChatResultDTO> GetTotalConversationsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<CustomerSatisfactionRatingsDTO> GetCustomerSatisfactionRatingsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<ChatVolumeDTO>> GetChatVolumeAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<FileResult> ExportChatReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath);

        Task<ReportingKBResultDTO> GetTotalKBDataAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<KBContentDistributionRatingDTO>> GetContentDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<KBTrendsDTO>> GetKBTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<FileResult> ExportKBReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath);

        Task<ReportingTicketResultDTO> GetTotalTicketDataAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<TicketTrendsDTO>> GetTicketTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<List<TicketPriorityDistributionDTO>> GetPriorityDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO);

        Task<FileResult> ExportTicketReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath);

    }
}