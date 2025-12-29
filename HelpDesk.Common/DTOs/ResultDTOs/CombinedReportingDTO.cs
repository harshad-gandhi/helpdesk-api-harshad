namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class CombinedReportingDTO
    {
        public ReportingChatResultDTO? ReportingChatResultDTO { get; set; }

        public CustomerSatisfactionRatingsDTO? CustomerSatisfactionRatingsDTO { get; set; }

        public List<ChatVolumeDTO>? ChatVolumeDTOs { get; set; }
    }
}