using System.Net;
using Azure;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TicketController(ITicketService ticketService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly ITicketService _ticketService = ticketService;
    protected IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    #region Ticket

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] TicketCreateRequestDTO request)
    {

        long ticketId = await _ticketService.CreateTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, ticketId, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_TICKET"]]]);

    }

    [HttpPost("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTicket([FromBody] TicketUpdateRequestDTO request)
    {

        await _ticketService.UpdateTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_TICKET"]]]);

    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicket([FromBody] TicketDeleteRequestDTO request)
    {

        await _ticketService.DeleteTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_TICKET"]]]);

    }

    [HttpGet("get")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTicketById(long TicketId)
    {
        if (TicketId <= 0)
        {
            List<string> errors = [_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_TICKET_ID"]]];
            throw new ValidationException(errors);
        }

        TicketDetailsDto Ticket = await _ticketService.GetTicketByIdAsync(TicketId);

        if (Ticket.TicketInfo == null)
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["DATA_NOT_FOUND"]]);
        }
        else
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, Ticket);
        }
    }

    [HttpGet("tickets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilteredTickets([FromQuery] GetTicketsByProjectRequestDto request)
    {

        PagedTicketWithTagsDto pagedTickets = await _ticketService.GetPaginatedTicketsAsync(request);

        if (pagedTickets.Tickets.Count == 0)
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["DATA_NOT_FOUND"]]);
        }
        else
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, pagedTickets);
        }

    }

    [HttpGet("departments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepartments()
    {
        List<DropdownDTO> result = await _ticketService.GetDepartmentsAsync();

        return _responseService.GetSuccessResponse(
            HttpStatusCode.OK,
            result
        );
    }

    [HttpGet("projects")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProjectsByUserId(int userId)
    {
        if (userId <= 0)
        {
            List<string> errors =
             [_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_USER_ID"]]];
            throw new ValidationException(errors);
        }

        List<DropdownDTO> projects = await _ticketService.GetProjectsByUserIdAsync(userId);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, projects);
    }

    [HttpPost("update-multiple")]
    public async Task<IActionResult> UpdateMultipleTickets([FromBody] MultipleTicketUpdateDTO request)
    {
        await _ticketService.UpdateMultipleTicketsAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_TICKET"]]]);
    }

    [HttpGet("usersList")]
    public async Task<IActionResult> GetProjectMembersByProjectId([FromQuery] int projectId)
    {
        List<DepartmentUserListResponseDTO> users = await _ticketService.GetProjectMembersByProjectIdAsync(projectId);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, users);
    }

    #endregion
    #region Ticket Event

    [HttpPost("create-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTicketEvent([FromForm] TicketEventCreateRequestDTO request)
    {

        TicketEventWithAttachmentsDto eventDetails = await _ticketService.CreateTicketEventAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, eventDetails);

    }
    #endregion

    #region Ticket Tag

    [HttpPost("add-tag")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddTicketTag([FromBody] TicketTagCreateRequestDto request)
    {

        await _ticketService.AddTagToTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_APPLY_SUCCESSFUL", _localizer["FIELD_TAG"]]]);

    }

    [HttpPost("remove-tag")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicketTag([FromBody] TicketTagDeleteRequestDto request)
    {

        await _ticketService.DeleteTicketTagAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_REMOVAL_SUCCESSFUL", _localizer["FIELD_TAG"]]]);
    }

    #endregion
    #region  Ticket Warchers

    [HttpPost("add-watcher")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddTicketWatcher([FromBody] TicketWatcherCreateRequestDTO request)
    {

        await _ticketService.AddWatcherToTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_ADD_SUCCESSFUL", _localizer["FIELD_WATCHER"]]]);

    }

    [HttpPost("delete-watcher")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicketWatcher([FromBody] TicketWatcherDeleteRequestDto request)
    {

        await _ticketService.DeleteWatcherFromTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_REMOVAL_SUCCESSFUL", _localizer["FIELD_WATCHER"]]]);

    }

    #endregion
}
