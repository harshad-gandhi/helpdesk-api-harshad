namespace HelpDesk.Common.DTOs.ResultDTOs;
 
public class EmailOutboxResultDTO
{
    public int Id { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
}
 