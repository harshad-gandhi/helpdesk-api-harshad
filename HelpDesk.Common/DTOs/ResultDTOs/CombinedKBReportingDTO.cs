namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class CombinedKBReportingDTO
    {
        public ReportingKBResultDTO? ReportingKBResultDTO { get; set; }

        public List<KBContentDistributionRatingDTO>? KBContentDistributionRatingDTO { get; set; }

        public List<KBTrendsDTO>? KBTrendsDTOs { get; set; }
    }
}