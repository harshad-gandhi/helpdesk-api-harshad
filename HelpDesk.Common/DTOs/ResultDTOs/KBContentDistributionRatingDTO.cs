namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class KBContentDistributionRatingDTO
    {
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int ArticleCount { get; set; }

        public int TotalArticles { get; set; }

        public decimal ArticlePercentage { get; set; }
    }
}