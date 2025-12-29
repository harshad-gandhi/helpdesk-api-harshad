using System.Data;
using System.Text.Json;
using Dapper;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class TicketRepository(IDbConnectionFactory connectionFactory) : ITicketRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    public async Task<TicketCreateResponseDTO?> CreateTicketAsync(TicketCreateRequestDTO request)
    {
        const string spName = "usp_ticket_create";

        DynamicParameters parameters = new();
        parameters.Add("@TicketForwardingEmail", request.TicketForwardingEmail);
        parameters.Add("@ProjectId", request.ProjectId);
        parameters.Add("@PersonId", request.PersonId);
        parameters.Add("@Email", request.Email);
        parameters.Add("@FirstName", request.FirstName);
        parameters.Add("@LastName", request.LastName);
        parameters.Add("@ChatSessionId", request.ChatSessionId);
        parameters.Add("@AssignedTo", request.AssignedTo);
        parameters.Add("@AssignedDepartmentId", request.AssignedDepartmentId);
        parameters.Add("@TicketNumber", request.TicketNumber);
        parameters.Add("@Subject", request.Subject);
        parameters.Add("@Description", request.Description);
        parameters.Add("@Status", request.Status);
        parameters.Add("@Priority", request.Priority);
        parameters.Add("@Source", request.Source);
        parameters.Add("@CreatedBy", request.CreatedBy);

        TicketCreateResponseDTO? result = await _baseRepository.QueryFirstOrDefaultAsync<TicketCreateResponseDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<int> UpdateTicketAsync(TicketUpdateRequestDTO request)
    {
        const string spName = "usp_ticket_update";

        object? changedValue = request.ChangedValue;
        if (changedValue is JsonElement jsonElement)
        {
            changedValue = jsonElement.ToString();
        }

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@ColumnCode", request.ColumnCode);
        parameters.Add("@ChangedValue", changedValue);
        parameters.Add("@UpdatedBy", request.UpdatedBy);
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

    public async Task<int> UpdateMultipleTicketsAsync(MultipleTicketUpdateDTO request)
    {
        const string spName = "usp_ticket_multiple_update";

        // Convert TicketIds to Table-Valued Parameter
        DataTable ticketIdsTable = new();
        ticketIdsTable.Columns.Add("Id", typeof(int));
        foreach (var id in request.TicketIds)
            ticketIdsTable.Rows.Add(id);

        DynamicParameters parameters = new();
        parameters.Add("@TicketIds", ticketIdsTable.AsTableValuedParameter("dbo.IntList"));
        parameters.Add("@ParentTicketId", request.ParentTicketId);
        parameters.Add("@UpdatedBy", request.UpdatedBy);
        parameters.Add("@ActionType", request.ActionType);
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


    public async Task<int> DeleteTicketAsync(TicketDeleteRequestDTO request)
    {
        const string spName = "usp_ticket_delete";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", request.TicketId);
        parameters.Add("@UpdatedBy", request.UpdatedBy);

        int result = await _baseRepository.QueryFirstOrDefaultAsync<int>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<TicketDetailsFlatDto> GetTicketByIdAsync(long ticketId)
    {
        const string spName = "usp_Ticket_Get_By_Id";

        DynamicParameters parameters = new();
        parameters.Add("@TicketId", ticketId);

        TicketDetailsFlatDto result = await _baseRepository.QueryMultipleAsync<TicketDetailsFlatDto>(
            spName,
            parameters,
            async grid =>
            {
                TicketInfoDto? ticketInfo = await grid.ReadFirstOrDefaultAsync<TicketInfoDto>();
                IEnumerable<TagResponseDTO> tags = await grid.ReadAsync<TagResponseDTO>();
                IEnumerable<TicketEventWithAttachmentDto> eventsWithAttachments = await grid.ReadAsync<TicketEventWithAttachmentDto>();
                IEnumerable<TicketEventDto> eventsWithoutAttachments = await grid.ReadAsync<TicketEventDto>();
                IEnumerable<TicketWatcherDto> watchers = await grid.ReadAsync<TicketWatcherDto>();

                return new TicketDetailsFlatDto
                {
                    TicketInfo = ticketInfo,
                    Tags = tags,
                    EventsWithAttachments = eventsWithAttachments,
                    EventsWithoutAttachments = eventsWithoutAttachments,
                    Watchers = watchers
                };
            },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<(IEnumerable<TicketDto> Tickets, IEnumerable<TicketTagDto> Tags, int TotalCount)> GetPaginatedTicketsAsync(GetTicketsByProjectRequestDto request)
    {
        const string spName = "usp_tickets_get_by_filters";

        DataTable tagIdsTable = new();
        tagIdsTable.Columns.Add("Id", typeof(int));
        if (request.TagIds != null)
        {
            foreach (var id in request.TagIds)
                tagIdsTable.Rows.Add(id);
        }

        DynamicParameters parameters = new();
        parameters.Add("@ProjectId", request.ProjectId);
        parameters.Add("@UserId", request.UserId);
        parameters.Add("@Category", request.Category);
        parameters.Add("@Assignee", request.Assignee);
        parameters.Add("@Status", request.Status);
        parameters.Add("@Priority", request.Priority);
        parameters.Add("@Department", request.Department);
        parameters.Add("@TagIds", tagIdsTable.AsTableValuedParameter("dbo.IntList"));
        parameters.Add("@StartDate", request.StartDate);
        parameters.Add("@EndDate", request.EndDate);
        parameters.Add("@Search", request.Search);
        parameters.Add("@PageSize", request.PageSize);
        parameters.Add("@PageNumber", request.PageNumber);
        parameters.Add("@SortBy", request.SortBy);
        parameters.Add("@SortDirection", request.SortDirection);

        (IEnumerable<TicketDto>, IEnumerable<TicketTagDto>, int) result = await _baseRepository.QueryMultipleAsync(
            spName,
            parameters,
            async grid =>
            {
                int totalCount = (await grid.ReadAsync<int>()).FirstOrDefault();
                IEnumerable<TicketDto> tickets = await grid.ReadAsync<TicketDto>();
                IEnumerable<TicketTagDto> tags = await grid.ReadAsync<TicketTagDto>();
                return (tickets, tags, totalCount);
            },
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task<List<UserListResponseDTO>> GetUserListAsync(int projectId)
        {
            const string spName = "usp_project_members_get_by_project_id";

            DynamicParameters parameters = new();
            parameters.Add("@ProjectId", projectId);

            List<UserListResponseDTO> users = (await _baseRepository.QueryAsync<UserListResponseDTO>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            return users;
        }


}
