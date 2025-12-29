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

public class TicketTagController(ITicketTagService ticketTagService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly ITicketTagService _ticketTagService = ticketTagService;
    protected IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddTicketTag([FromBody] TicketTagCreateRequestDto request)
    {

        await _ticketTagService.AddTagToTicketAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_APPLY_SUCCESSFUL", _localizer["FIELD_TAG"]]]);

    }

    [HttpPost("delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTicketTag([FromBody] TicketTagDeleteRequestDto request)
    {

        await _ticketTagService.DeleteTicketTagAsync(request);

        return _responseService.GetSuccessResponse(HttpStatusCode.OK, null, [_localizer["ENTITY_REMOVAL_SUCCESSFUL", _localizer["FIELD_TAG"]]]);
    }
}
