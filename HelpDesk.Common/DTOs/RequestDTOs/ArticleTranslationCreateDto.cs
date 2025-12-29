using static HelpDesk.Common.Enums.Enumerations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ArticleTranslationCreateDto
{
     public int ArticleId { get; set; }
    public Language Language { get; set; }
    public string Title { get; set; }
    public string Subtitle{ get; set; }
    public string Content { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public string Slug { get; set; }
}
