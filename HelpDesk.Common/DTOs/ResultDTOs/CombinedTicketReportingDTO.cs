namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class CombinedTicketReportingDTO
    {
        public ReportingTicketResultDTO? ReportingTicketResultDTO { get; set; }

        public List<TicketPriorityDistributionDTO>? TicketPriorityDistributionDTO { get; set; }

        public List<TicketTrendsDTO>? TicketTrendsDTOs { get; set; }
    }
}