namespace HelpDesk.Common.DTOs.ResultDTOs;

public class BackupCodeResultDTO
{
    public int Id { get; set; }
    public string BackupCodeHash { get; set; } = default!;
}
