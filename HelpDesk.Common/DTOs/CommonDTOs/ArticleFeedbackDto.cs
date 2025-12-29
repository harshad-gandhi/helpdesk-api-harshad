namespace HelpDesk.Common.DTOs.CommonDTOs;

public class ArticleFeedbackDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int PersonId { get; set; }
    public bool IsHelpful { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
