using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IEmailOutboxRepository
{
    Task<List<EmailOutboxResultDTO>> GetPendingEmailsAsync();
    Task UpdateEmailStatusAsync(int id, bool isSuccess, string? errorMessage);
}
