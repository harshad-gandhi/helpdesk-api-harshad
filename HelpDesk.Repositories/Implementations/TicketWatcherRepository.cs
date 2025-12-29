using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class TicketWatcherRepository(IDbConnectionFactory connectionFactory) : ITicketWatcherRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<int> CreateTicketWatcherAsync(TicketWatcherCreateRequestDTO request)
    {
        const string spName = "usp_ticket_watcher_create";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@ProjectMemberId", request.ProjectMemberId);
        parameters.Add("@CreatedBy", request.CreatedBy);
        parameters.Add("@EventType", request.EventType);
        parameters.Add("@PerformerType", request.PerformerType);
        parameters.Add("@EventText", request.EventText);
        parameters.Add("@Metadata", request.Metadata);
        parameters.Add("@IsInternal", request.IsInternal);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<int> DeleteTicketWatcherAsync(TicketWatcherDeleteRequestDto request)
    {
        const string spName = "usp_ticket_watcher_delete";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@ProjectMemberId", request.ProjectMemberId);
        parameters.Add("@DeletedBy", request.DeletedBy);
        parameters.Add("@EventType", request.EventType);
        parameters.Add("@PerformerType", request.PerformerType);
        parameters.Add("@EventText", request.EventText);
        parameters.Add("@Metadata", request.Metadata);
        parameters.Add("@IsInternal", request.IsInternal);

        int result = await _baseRepository.ExecuteScalarAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
}
