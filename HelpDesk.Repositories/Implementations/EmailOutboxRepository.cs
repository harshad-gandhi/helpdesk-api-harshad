using Dapper;
using System.Data;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository implementation for managing email outbox operations.
/// </summary>
public class EmailOutboxRepository(IDbConnectionFactory connectionFactory) : IEmailOutboxRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves all pending emails that are yet to be sent.
    /// </summary>
    /// <returns>A list of <see cref="EmailOutboxResultDTO"/> representing the pending emails.</returns>
    public async Task<List<EmailOutboxResultDTO>> GetPendingEmailsAsync()
    {
        const string spName = "usp_email_outbox_get_pending";

        IEnumerable<EmailOutboxResultDTO> unsentEmailList = await _baseRepository.QueryAsync<EmailOutboxResultDTO>(
            spName,
            commandType: CommandType.StoredProcedure
        );

        return unsentEmailList.AsList();
    }

    /// <summary>
    /// Updates the status of an email in the outbox after attempting to send it.
    /// </summary>
    /// <param name="id">The ID of the email to update.</param>
    /// <param name="isSuccess">Indicates whether the email was successfully sent.</param>
    /// <param name="errorMessage">Optional error message if sending failed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateEmailStatusAsync(int id, bool isSuccess, string? errorMessage)
    {
        const string spName = "usp_email_outbox_update_status";

        DynamicParameters parameters = new();
        parameters.Add("@Id", id);
        parameters.Add("@IsSuccess", isSuccess);
        parameters.Add("@ErrorMessage", errorMessage);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );
    }

}
