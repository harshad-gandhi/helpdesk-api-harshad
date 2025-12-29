using System.Net;
using System.Security.Claims;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Crypto.Engines;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ChatController(IChatService chatService,
        IResponseService<object> responseService,
        IStringLocalizer<Messages> localizer) : ControllerBase
    {
        private readonly IChatService _chatService = chatService;
        private readonly IResponseService<object> _responseService = responseService;
        private readonly IStringLocalizer<Messages> _localizer = localizer;

        /// <summary>
        /// Creates a new chat session.
        /// </summary>
        /// <param name="chatSessionCreateDto">The data transfer object containing details required to create a chat session.</param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="ChatSessionDto"/> 
        /// along with a success response and HTTP status code 201 (Created).
        /// </returns>
        #region ChatSession
        [HttpPost("chatsession/create")]
        public async Task<IActionResult> CreateChatSessionAsync([FromBody] ChatSessionsCreateDto chatSessionCreateDto)
        {
            if (string.IsNullOrEmpty(chatSessionCreateDto.ChatIpAddress))
            {
                string? forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                chatSessionCreateDto.ChatIpAddress = !string.IsNullOrEmpty(forwardedHeader) ? forwardedHeader.Split(',').First().Trim() : HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            (ChatSessionDto chatSessionDto, bool existingchat) = await _chatService.CreateChatSessionAsync(chatSessionCreateDto);
            if (existingchat)
            {
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessionDto, [_localizer["ACTIVE_CHAT_ALREADY_EXISTS_WITH_ID", chatSessionDto.Id.ToString()]]);
            }
            return _responseService.GetSuccessResponse(HttpStatusCode.Created, chatSessionDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Updates an existing chat session.
        /// </summary>
        /// <param name="chatSessionDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="ChatSessionDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chatsession/update")]
        public async Task<IActionResult> UpdateChatSessionAsync([FromBody] ChatSessionDto chatSessionDto)
        {
            ChatSessionDto chatSessionDtoresponse = await _chatService.UpdateChatSessionAsync(chatSessionDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessionDtoresponse, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Deletes a chat session by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpDelete("chatsession/{id:long}")]
        public async Task<IActionResult> DeleteChatSessionAsync(long id)
        {
            bool isDeleted = await _chatService.DeleteChatSessionAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Retrieves a chat session by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="ChatSessionsListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chatsession/{id:int}")]
        public async Task<IActionResult> GetChatSessionByIdAsync(long id)
        {
            ChatSessionsListDto chatSession = await _chatService.GetChatSessionByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSession, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Retrieves all chat sessions based on the provided filter criteria.
        /// </summary>
        /// <param name="chatSessionFilterDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a paged result of <see cref="ChatSessionsListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpGet("chatsession/getbyfilter")]
        public async Task<IActionResult> GetAllChatSessionsByFilterAsync([FromQuery] ChatSessionsFilterDto chatSessionFilterDto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
            chatSessionFilterDto.UserId = int.Parse(userIdStr);
            PagedResult<ChatSessionsListDto> chatSessions = await _chatService.GetAllChatSessionsByFilterAsync(chatSessionFilterDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessions, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Retrieves recent chat sessions for a specific person within a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="personId"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of <see cref="ChatSessionsListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chatsession/getrecentbyperson")]
        public async Task<IActionResult> GetAllRecentChatSessionsByPersonAsync([FromQuery] long projectId, [FromQuery] int personId)
        {
            List<ChatSessionsListDto> chatSessions = await _chatService.GetRecentChatSessionsAsync(projectId, personId);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessions, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Marks a chat session as trashed or untrashed.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inTrash"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the operation was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPatch("chatsession/trash/{id:int}")]
        public async Task<IActionResult> TrashOrUntrashChatSessionAsync(int id, [FromBody] bool inTrash)
        {
            bool isUpdated = await _chatService.ChatSessionMarkTrashAsync(
                id,
                inTrash,
                DateTimeOffset.UtcNow
            );

            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                isUpdated,
                [_localizer[inTrash ? "ENTITY_MARKED_TRASH" : "ENTITY_UNTRASHED", _localizer["FIELD_CHAT_SESSION"]]]
            );
        }

        /// <summary>
        /// Marks a chat session as spam or unspam.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSpam"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the operation was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPatch("chatsession/spam/{id:int}")]
        public async Task<IActionResult> SpamOrUnSpamChatSessionAsync(int id, [FromBody] bool isSpam)
        {
            bool isUpdated = await _chatService.ChatsessionMarkSpamAsync(
                id,
                isSpam,
                DateTimeOffset.UtcNow
            );

            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                isUpdated,
                [_localizer[isSpam ? "ENTITY_MARKED_SPAM" : "ENTITY_UNSPAMMED", _localizer["FIELD_CHAT_SESSION"]]]
            );
        }

        /// <summary>
        /// Retrieves all active chat sessions assigned to the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of active <see cref="ChatSessionsListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("my-active")]
        public async Task<IActionResult> GetMyAssignedActiveChatsAsync()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null)
            {
                return _responseService.GetErrorResponse(HttpStatusCode.Unauthorized, [_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]]);
            }
            var filterDto = new ChatSessionsFilterDto
            {
                UserId = int.Parse(userIdStr),
                IsAssignedToUserActive = true
            };
            var chatSessions = await _chatService.GetAllChatSessionsByFilterAsync(filterDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessions, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHAT_SESSION"]]]);
        }

        /// <summary>
        /// Changes the status of a chat session.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a message indicating the result of the operation,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chatsession/{id:int}/status/{status:int}")]
        public async Task<IActionResult> ChangeChatSessionStatusAsync(int id, int status)
        {
            var message = await _chatService.ChangeChatsessionStatusAsync(id, status);

            return _responseService.GetSuccessResponse(
                HttpStatusCode.OK,
                null,
                [message]
            );
        }

        #endregion

        #region ChatTransfers
        
        /// <summary>
        /// Creates a new chat transfer.
        /// </summary>
        /// <param name="chatTransferCreateDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="ChatTransfersDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chattransfer/create")]
        public async Task<IActionResult> CreateChatTransfersAsync([FromBody] ChatTransfersCreateDto chatTransferCreateDto)
        {
            ChatTransfersDto chatSessionDto = await _chatService.CreateChatTransfersAsync(chatTransferCreateDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessionDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_CHATS_TRANSFERS"]]]);
        }

        /// <summary>
        /// Updates an existing chat transfer.
        /// </summary>
        /// <param name="chatTransferDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="ChatTransfersDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chattransfer/update")]
        public async Task<IActionResult> UpdateChatTransfersAsync([FromBody] ChatTransfersDto chatTransferDto)
        {
            ChatTransfersDto chatTransferDtoResponse = await _chatService.UpdateChatTransfersAsync(chatTransferDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatTransferDto, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_CHATS_TRANSFERS"]]]);
        }

        /// <summary>
        /// Deletes a chat transfer by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpDelete("chattransfer/{id:long}")]
        public async Task<IActionResult> DeleteChatTransfersAsync(long id)
        {
            bool isDeleted = await _chatService.DeleteChatTransfersAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_CHATS_TRANSFERS"]]]);
        }
        
        /// <summary>
        /// Retrieves a chat transfer by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="ChatTransfersDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chattransfer/{id:int}")]
        public async Task<IActionResult> GetChatTransfersByIdAsync(int id)
        {
            ChatTransfersDto chatTransfer = await _chatService.GetChatTransfersByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatTransfer, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHATS_TRANSFERS"]]]);
        }

        /// <summary>
        /// Retrieves all chat transfers associated with a specific chat session ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of <see cref="ChatTransfersDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chattransfer/getbychatSession/{id:long}")]
        public async Task<IActionResult> GetAllChatTransfersByChatSessionIdAsync(long id)
        {
            List<ChatTransfersDto> chatTransfers = await _chatService.GetAllChatTransfersByChatSessionIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatTransfers, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHATS_TRANSFERS"]]]);
        }
        #endregion

        #region ChatsTagsMapping

        /// <summary>
        /// Creates a new chat-tags mapping.
        /// </summary>
        /// <param name="chatsTagsMappingCreateDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="ChatsTagsMappingDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chatstagsmapping/create")]
        public async Task<IActionResult> CreateChatsTagsMappingAsync([FromBody] ChatsTagsMappingCreateDto chatsTagsMappingCreateDto)
        {
            ChatsTagsMappingDto chatSessionDto = await _chatService.CreateChatsTagsMappingAsync(chatsTagsMappingCreateDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatSessionDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_CHATS_TAGS_MAPPING"]]]);
        }

        /// <summary>
        /// Updates an existing chat-tags mapping.
        /// </summary>
        /// <param name="chatsTagsMappingDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="ChatsTagsMappingDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chatstagsmapping/update")]
        public async Task<IActionResult> UpdateChatsTagsMppingAsync([FromBody] ChatsTagsMappingDto chatsTagsMappingDto)
        {
            ChatsTagsMappingDto chatsTagsMappingDtoRespons = await _chatService.UpdateChatTagsMappingAsync(chatsTagsMappingDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatsTagsMappingDtoRespons, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_CHATS_TAGS_MAPPING"]]]);
        }

        /// <summary>
        /// Deletes a chat-tags mapping by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpDelete("chatstagsmapping/{id:long}")]
        public async Task<IActionResult> DeleteChatsTagsMappingAsync(long id)
        {
            bool isDeleted = await _chatService.DeleteChatsTagsMappingAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_CHATS_TAGS_MAPPING"]]]);
        }

        /// <summary>
        /// Retrieves a chat-tags mapping by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="ChatsTagsMappingDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chatstagsmapping/{id:int}")]
        public async Task<IActionResult> GetChatsTagsMappingByIdAsync(int id)
        {
            ChatsTagsMappingDto chatsTagsMapping = await _chatService.GetChatsTagsMappingByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatsTagsMapping, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHATS_TAGS_MAPPING"]]]);
        }
        #endregion

        /// <summary>
        /// Creates a new chat message.
        /// </summary>
        /// <param name="chatMessagesCreateDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="ChatMessagesDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        #region ChatMessages
        [HttpPost("chatmessage/create")]

        public async Task<IActionResult> CreateChatMessagesAsync([FromForm] ChatMessagesCreateDto chatMessagesCreateDto)
        {
            ChatMessagesDto chatMessagesDto = await _chatService.CreateChatMessagesAsync(chatMessagesCreateDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatMessagesDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_CHATS_MESSAGES"]]]);
        }

        /// <summary>
        /// Updates an existing chat message.
        /// </summary>
        /// <param name="chatMessagesDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="ChatMessagesDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpPost("chatmessage/update")]
        public async Task<IActionResult> UpdateChatMessagesAsync([FromBody] ChatMessagesDto chatMessagesDto)
        {
            ChatMessagesDto chatMessagesDtoResponse = await _chatService.UpdateChatMessagesAsync(chatMessagesDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatMessagesDtoResponse, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_CHATS_MESSAGES"]]]);
        }

        /// <summary>
        /// Deletes a chat message by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpDelete("chatmessage/{id:long}")]
        public async Task<IActionResult> DeleteChatMessagesAsync(long id)
        {
            bool isDeleted = await _chatService.DeleteChatMessagesAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_CHATS_MESSAGES"]]]);
        }

        /// <summary>
        /// Retrieves a chat message by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="ChatMessagesDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chatmessage/{id:int}")]
        public async Task<IActionResult> GetChatMessagesByIdAsync(int id)
        {
            ChatMessagesDto chatMessage = await _chatService.GetChatMessagesByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatMessage, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHATS_MESSAGES"]]]);
        }

        /// <summary>
        /// Retrieves all chat messages based on the provided filter criteria.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchTerm"></param>
        /// <param name="isAgent"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of <see cref="ChatMessagesDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("chatmessage/getbyfilter")]
        public async Task<IActionResult> GetAllChatMessagesByFilterAsync([FromQuery] long id, [FromQuery] string? searchTerm, [FromQuery] bool isAgent = false)
        {
            List<ChatMessagesDto> chatMessages = await _chatService.GetAllChatMessagesByFilterAsync(id, searchTerm, isAgent);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, chatMessages, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_CHATS_MESSAGES"]]]);
        }
        #endregion
    }

}

