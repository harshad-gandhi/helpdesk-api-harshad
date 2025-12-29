using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class TicketTagRepository(IDbConnectionFactory connectionFactory) : ITicketTagRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<int> CreateTicketTagMappingAsync(TicketTagCreateRequestDto request)
    {
        const string spName = "usp_ticket_tags_mapping_create";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@TagId", request.TagId);
        parameters.Add("@TagName", request.TagName);
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

    public async Task<int> DeleteTicketTagMappingAsync(TicketTagDeleteRequestDto request)
    {
        const string spName = "usp_ticket_tags_mapping_delete";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@TagId", request.TagId);
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
