namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ArticleFeedbackCreateDto
{
    public int ArticleId { get; set; }
    public int PersonId { get; set; }
    public bool IsHelpful { get; set; }
    public string? Comment { get; set; }
}
