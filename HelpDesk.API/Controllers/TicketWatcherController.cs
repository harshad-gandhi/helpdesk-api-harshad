using System.Net;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TicketWatcherController(ITicketWatcherService ticketWatcherService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly ITicketWatcherService _ticketWatcherService = ticketWatcherService;
    protected IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddTicketWatcher([FromBody] TicketWatcherCreateRequestDTO request)
    {

        await _ticketWatcherService.AddWatcherToTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_ADD_SUCCESSFUL", _localizer["FIELD_WATCHER"]]]);

    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicketWatcher([FromBody] TicketWatcherDeleteRequestDto request)
    {

        await _ticketWatcherService.DeleteWatcherFromTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_REMOVAL_SUCCESSFUL", _localizer["FIELD_WATCHER"]]]);

    }
}
