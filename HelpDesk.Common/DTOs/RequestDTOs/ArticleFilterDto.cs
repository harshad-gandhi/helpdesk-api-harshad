namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ArticleFilterDto
{
    public int? ProjectId { get; set; }
    public string? Search { get; set; }
    public int? Visibility { get; set; }
    public int? Status { get; set; }
    public string? AuthorName { get; set; }
    public string? CategoryName { get; set; }
}
