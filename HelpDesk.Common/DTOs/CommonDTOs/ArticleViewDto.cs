namespace HelpDesk.Common.DTOs.CommonDTOs;

public class ArticleViewDto
{
    public int Id {get; set; }
    public int ArticleId { get; set; }
    public int PersonId { get; set; }
    public string IpAddress { get; set; }
    public DateTimeOffset ViewAt { get; set; }
}
