namespace HelpDesk.Common.DTOs.CommonDTOs;

public class KbSearchAnalyticsDto
{
    public int Id { get; set; }
    public int? ProjectId { get; set; }
    public string Keyword { get; set; }
    public int? ResultCount { get; set; }
}
