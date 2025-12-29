using System.Net;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TagsController(ITagService tagService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly ITagService _tagService = tagService;
    protected IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags([FromQuery] TagGetRequestDto request)
    {

        IEnumerable<TagResponseDTO> result = await _tagService.GetTagsAsync(request);
        
        if (result is null || !result.Any())
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["DATA_NOT_FOUND"]]);
        }
        else
        {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, result);
        }
    }

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateRequestDTO request)
    {

        TagCreateResponseDTO result = await _tagService.CreateTagAsync(request);
        
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result.TagId, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_TAG"]]]);
        
    }
}
