using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;
using Newtonsoft.Json;

namespace HelpDesk.Repositories.Implementations;

public class TicketEventRepository(IDbConnectionFactory connectionFactory) : ITicketEventRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<(int statusCode, IEnumerable<TicketEventWithAttachmentDto> eventDetails)> CreateTicketEventAsync(TicketEventCreateRequestDTO request)
    {
        const string spName = "usp_ticket_event_create";

        string attachmentsJson = "";
        if (request.EventType == 1 && request.Attachments?.Count > 0)
        {
            attachmentsJson = JsonConvert.SerializeObject(request.Attachments);
        }

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@EventType", request.EventType);
        parameters.Add("@PerformerType", request.PerformerType);
        parameters.Add("@EventText", request.EventText);
        parameters.Add("@Metadata", request.Metadata);
        parameters.Add("@IsInternal", request.IsInternal);
        parameters.Add("@CreatedBy", request.CreatedBy);
        parameters.Add("@Attachments", attachmentsJson);
        parameters.Add("@EmailMessageId", request.EmailMessageId);

         var result = await _baseRepository.QueryMultipleAsync<(int, IEnumerable<TicketEventWithAttachmentDto>)>(
        spName,
        parameters,
        async grid =>
        {
            var data1 = await grid.ReadFirstAsync<(int, long)>();
            int statusCode = await grid.ReadFirstAsync<int>();

            if (statusCode == 1)
            {
                var data = await grid.ReadAsync<TicketEventWithAttachmentDto>();
                return (statusCode, data);
            }
            else 
            {
                return (statusCode, Enumerable.Empty<TicketEventWithAttachmentDto>());
            }
        },
        commandType: CommandType.StoredProcedure
    );

        return result;
    }
}
