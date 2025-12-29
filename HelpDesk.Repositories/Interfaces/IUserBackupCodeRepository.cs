using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IUserBackupCodeRepository
{
    Task<List<BackupCodeResultDTO>> GetUnusedBackupCodesAsync(int userId);
    Task MarkBackupCodeAsUsedAsync(int codeId);
}
