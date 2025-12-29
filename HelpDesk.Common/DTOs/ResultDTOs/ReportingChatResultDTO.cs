namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class ReportingChatResultDTO
    {
        public int TotalConversations { get; set; }

        public int AvgResponseTimeSeconds { get; set; }

        public double AvgSatisfactionScore { get; set; }

        public double ResolutionRatePercentage { get; set; }

        public double HighRatingPercentage { get; set; }

        public int PrevTotalConversations { get; set; }

        public int CurTotalConversations { get; set; }

        public double ConversationGrowthPercentage { get; set; }

        public string? ProjectName { get; set; }
    }
}