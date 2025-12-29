namespace HelpDesk.Common.DTOs.ResultDTOs;

public class EnableTwoFAResultDTO
{
    public int StatusCode { get; set; }
    public string Email { get; set; } = string.Empty;
}
