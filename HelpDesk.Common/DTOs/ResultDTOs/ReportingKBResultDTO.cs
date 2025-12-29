namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class ReportingKBResultDTO
    {
        public int TotalArticleViews { get; set; }

        public int TotalSearches { get; set; }

        public double AvgRating { get; set; }

        public double ViewGrowthPercentage { get; set; }

        public double SearchGrowthPercentage { get; set; }

        public int PeakViewHour { get; set; }

        public int PeakSearchHour { get; set; }

        public string? MostViewedArticle { get; set; }

        public double MostViewedArticlePercentage { get; set; }

        public string? ProjectName { get; set; }
    }
}