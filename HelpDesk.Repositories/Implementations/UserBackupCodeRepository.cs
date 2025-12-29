using Dapper;
using System.Data;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository for managing user backup codes used in two-factor authentication (2FA).
/// </summary>
public class UserBackupCodeRepository(IDbConnectionFactory connectionFactory) : IUserBackupCodeRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves all unused backup codes for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose unused backup codes are being fetched.</param>
    /// <returns>
    /// A list of <see cref="BackupCodeResultDTO"/> containing the unused backup codes.
    /// </returns>
    public async Task<List<BackupCodeResultDTO>> GetUnusedBackupCodesAsync(int userId)
    {
        const string spName = "usp_user_backup_codes_get_unused_backup_codes";

        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId);

        IEnumerable<BackupCodeResultDTO>? codes = await _baseRepository.QueryAsync<BackupCodeResultDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return codes.AsList();
    }

    /// <summary>
    /// Marks a backup code as used after it has been consumed for login or verification.
    /// </summary>
    /// <param name="codeId">The ID of the backup code to mark as used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task MarkBackupCodeAsUsedAsync(int codeId)
    {
        const string spName = "usp_user_backup_codes_update_used_backup_code";

        DynamicParameters parameters = new();
        parameters.Add("@Id", codeId);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }
}