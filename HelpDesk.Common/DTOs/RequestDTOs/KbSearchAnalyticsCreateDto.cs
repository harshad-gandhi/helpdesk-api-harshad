namespace HelpDesk.Common.DTOs.RequestDTOs;

public class KbSearchAnalyticsCreateDto
{
    public int? ProjectId { get; set; }
    public string Keyword { get; set; }
    public int? ResultCount { get; set; }
}
