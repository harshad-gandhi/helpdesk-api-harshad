using System.ComponentModel.DataAnnotations.Schema;
using static HelpDesk.Common.Enums.Enumerations;

namespace HelpDesk.Common.DTOs.CommonDTOs;

public class ArticleDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ArticleContent { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public Status Status { get; set; }
    public Visibility Visibility { get; set; }
    public Language Language { get; set; }

    public int? SortOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public int CreatedBy { get; set; }
    public string? AuthorName { get; set; } = string.Empty;
    public int? UpdatedBy { get; set; }
    public string? CategoriesCsv { get; set; }
    public string? RelatedArticlesCsv { get; set; }
    [NotMapped]
    public List<string> Categories =>
        string.IsNullOrEmpty(CategoriesCsv)
            ? new List<string>()
            : CategoriesCsv.Split(',').ToList();
    [NotMapped]
    public List<string> RelatedArticles =>
        string.IsNullOrEmpty(RelatedArticlesCsv)
            ? new List<string>()
            : RelatedArticlesCsv.Split(',').ToList();
}
