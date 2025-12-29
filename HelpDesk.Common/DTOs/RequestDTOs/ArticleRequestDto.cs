using static HelpDesk.Common.Enums.Enumerations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ArticleRequestDto
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Status Status { get; set; } 
    public  Visibility Visibility { get; set; }
    public  Language Language { get; set; }
    public int? SortOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int CreatedBy { get; set; }
    public List<int>? CategoryIds { get; set; }
    public List<int>? RelatedArticleIds { get; set; }
}
