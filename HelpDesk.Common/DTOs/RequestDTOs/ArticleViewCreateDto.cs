namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ArticleViewCreateDto
{
    public int ArticleId { get; set; }
    public int PersonId { get; set; }
    public string IpAddress { get; set; }
}
