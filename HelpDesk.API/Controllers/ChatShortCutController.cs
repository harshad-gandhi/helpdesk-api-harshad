using System.Net;
using Microsoft.AspNetCore.Mvc;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Constants;
using HelpDesk.Common.DTOs.RequestDTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.Resources;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route(SystemConstant.API_CHAT_SHORTCUT_MESSAGES)]
    [Produces(SystemConstant.APPLICATION_JSON)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class ChatShortCutController(IChatShortCutService chatShortCutService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
    {
        private readonly IChatShortCutService _chatShortCutService = chatShortCutService;

        private readonly IResponseService<object> _responseService = responseService;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        /// <summary>
        /// Retrieves the list of chat shortcuts for a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project for which to fetch chat shortcuts.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of <see cref="ChatShortCutResponseDTO"/> objects wrapped in a success response.
        /// </returns>
        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetChatShortCutAsync([FromRoute] int projectId)
        {
            List<ChatShortCutResponseDTO> chatShortCutResponseDTOs = await _chatShortCutService.GetChatShortCutAsync(projectId);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatShortCutResponseDTOs);
        }

        /// <summary>
        /// Creates a new chat shortcut for the authenticated user.
        /// </summary>
        /// <param name="chatShortCutCreateRequestDTO">The data transfer object containing the details of the chat shortcut to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the created <see cref="ChatShortCutResponseDTO"/> 
        /// wrapped in a success response, or an error response if the user ID is invalid or missing.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateChatShortCutAsync([FromBody] ChatShortCutCreateRequestDTO chatShortCutCreateRequestDTO)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId) || userId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }
            chatShortCutCreateRequestDTO.UserId = userId;

            ChatShortCutResponseDTO chatShortCutResponseDTO = await _chatShortCutService.CreateChatShortCutAsync(chatShortCutCreateRequestDTO);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatShortCutResponseDTO);
        }

        /// <summary>
        /// Updates an existing chat shortcut for the authenticated user.
        /// </summary>
        /// <param name="chatShortCutUpdateRequestDTO">The data transfer object containing the updated details of the chat shortcut.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated <see cref="ChatShortCutResponseDTO"/> 
        /// wrapped in a success response, or an error response if the user ID is invalid or missing.
        /// </returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateChatShortCutAsync([FromBody] ChatShortCutUpdateRequestDTO chatShortCutUpdateRequestDTO)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId) || userId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }
            chatShortCutUpdateRequestDTO.UserId = userId;

            ChatShortCutResponseDTO chatShortCutResponseDTO = await _chatShortCutService.UpdateChatShortCutAsync(chatShortCutUpdateRequestDTO);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatShortCutResponseDTO);
        }

        /// <summary>
        /// Deletes a chat shortcut for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the deleted <see cref="ChatShortCutResponseDTO"/> 
        /// wrapped in a success response, or an error response if the user ID is invalid or missing.
        /// </returns>
        [HttpPatch("delete/{id}")]
        public async Task<IActionResult> DeleteChatShortCutAsync([FromRoute] int id)
        {

            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId) || userId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = await _chatShortCutService.DeleteChatShortCutAsync(id, userId);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatShortCutResponseDTO);

        }

        /// <summary>
        /// Toggles the visibility (public/private) of a chat shortcut for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut whose visibility is to be toggled.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated <see cref="ChatShortCutResponseDTO"/> 
        /// wrapped in a success response, or an error response if the user ID is invalid or missing.
        /// </returns>
        [HttpPatch("visibility/{id}")]
        public async Task<IActionResult> ToggleChatShortCutVisibilityAsync([FromRoute] int id)
        {

            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId) || userId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = await _chatShortCutService.ToggleChatShortCutVisibilityAsync(id, userId);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatShortCutResponseDTO);
        }

    }
}