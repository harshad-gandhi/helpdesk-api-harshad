using System.Net;
using Microsoft.AspNetCore.Mvc;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Constants;
using HelpDesk.Common.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.SignalR;
using HelpDesk.Services.Hubs;
using HelpDesk.Common.DTOs.ResultDTOs;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using HelpDesk.Common.Resources;
using Microsoft.AspNetCore.Authorization;

namespace HelpDesk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route(SystemConstant.API_DIRECT_MESSAGES)]
    [Produces(SystemConstant.APPLICATION_JSON)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class DirectMessageController(IDirectMessageService directMessageService, IResponseService<object> responseService, IHubContext<DirectMessageHub> chatHub, IStringLocalizer<Messages> localizer) : ControllerBase
    {

        private readonly IDirectMessageService _directMessageService = directMessageService;

        private readonly IResponseService<object> _responseService = responseService;

        private readonly IHubContext<DirectMessageHub> _chatHub = chatHub;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        /// <summary>
        /// Sends a direct message from the authenticated user to the specified receiver.
        /// </summary>
        /// <param name="directMessageSendRequestDTO">
        /// The message payload including receiver information, text, and optional attachment.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the sent message response DTO
        /// on success.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SendDirectMessageAsync([FromForm] DirectMessageSendRequestDTO directMessageSendRequestDTO)
        {

            string? senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(senderIdStr) || !int.TryParse(senderIdStr, out int senderId) || senderId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }

            directMessageSendRequestDTO.SenderId = senderId;

            DirectMessageSendResponseDTO directMessageSendResponseDTO = await _directMessageService.SendDirectMessageAsync(directMessageSendRequestDTO);

            // Send to receiver's active connections
            await _chatHub.Clients.User(directMessageSendRequestDTO.ReceiverId.ToString())
                .SendAsync("ReceiveMessage", directMessageSendResponseDTO);

            // Echo back to sender’s connections too
            await _chatHub.Clients.User(directMessageSendRequestDTO.SenderId.ToString())
                .SendAsync("ReceiveMessage", directMessageSendResponseDTO);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs_receiver = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageSendRequestDTO.ReceiverId);

            // Update the recent message list of the receiver
            await _chatHub.Clients.User(directMessageSendRequestDTO.ReceiverId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs_receiver);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs_sender = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageSendRequestDTO.SenderId);

            // Update the recent message list of the sender
            await _chatHub.Clients.User(directMessageSendRequestDTO.SenderId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs_sender);

            return _responseService.GetSuccessResponse(statusCode: HttpStatusCode.OK, directMessageSendResponseDTO);
        }

        /// <summary>
        /// Retrieves direct messages exchanged between the authenticated user and the specified receiver.
        /// </summary>
        /// <param name="directMessagesGetRequestDTO">
        /// The request DTO containing receiver information and pagination/filter options.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of direct messages.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetDirectMessagesBetweenUsersAsync([FromQuery] DirectMessagesGetRequestDTO directMessagesGetRequestDTO)
        {

            string? senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(senderIdStr) || !int.TryParse(senderIdStr, out int senderId) || senderId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }

            directMessagesGetRequestDTO.SenderId = senderId;

            List<DirectMessageGetResponseDTO>? directMessageGetResponseDTOs = await _directMessageService.GetDirectMessagesBetweenUsersAsync(directMessagesGetRequestDTO);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageGetResponseDTOs);
        }

        /// <summary>
        /// Retrieves recent direct messages exchanged between the authenticated user and the all receiver.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of recent direct messages.
        /// </returns>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentDirectMessagesBetweenUsersAsync()
        {
            string? senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(senderIdStr) || !int.TryParse(senderIdStr, out int senderId) || senderId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }
            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(senderId);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessagesRecentMessagesResponseDTOs);
        }

        /// <summary>
        /// Marks a specific direct message as read and notifies both sender and receiver via SignalR.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated message read status.
        /// </returns>
        [HttpPatch("read/{messageId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkDirectMessageAsReadAsync([FromRoute] long messageId)
        {
            DirectMessageMarkAsReadResponseDTO directMessageMarkAsReadResponseDTO = await _directMessageService.MarkDirectMessageAsReadAsync(messageId);

            // Notify the sender about the read status
            await _chatHub.Clients.User(directMessageMarkAsReadResponseDTO.SenderId.ToString())
                .SendAsync("MessageMarkAsRead", directMessageMarkAsReadResponseDTO);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageMarkAsReadResponseDTO.ReceiverId);

            // Update the unread count for the receiver
            await _chatHub.Clients.User(directMessageMarkAsReadResponseDTO.ReceiverId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageMarkAsReadResponseDTO);
        }

        /// <summary>
        /// Marks all direct messages for the current user as read and notifies the senders via SignalR.
        /// </summary>
        /// <param name="directMessageMarkAllAsReadRequestDTO">The request containing necessary information to mark messages as read.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of messages that were marked as read.
        /// </returns>
        [HttpPatch("read")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAllDirectMessageAsReadAsync([FromBody] DirectMessageMarkAllAsReadRequestDTO directMessageMarkAllAsReadRequestDTO)
        {

            string? receiverIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(receiverIdStr) || !int.TryParse(receiverIdStr, out int receiverId) || receiverId <= 0)
            {
                LocalizedString? errorMessage = _localizer["INVALID_OR_MISSING_USER_ID"];
                return _responseService.GetErrorResponse(
                    statusCode: HttpStatusCode.Unauthorized,
                    errors: [errorMessage]
                );
            }

            directMessageMarkAllAsReadRequestDTO.ReceiverId = receiverId;

            List<DirectMessageResultDTO> directMessageResultDTOs = await _directMessageService.MarkAllDirectMessageAsReadAsync(directMessageMarkAllAsReadRequestDTO);

            foreach (DirectMessageResultDTO? msg in directMessageResultDTOs)
            {
                await _chatHub.Clients.User(msg.SenderId.ToString())
                    .SendAsync("MessageMarkAsRead", msg);
            }

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageMarkAllAsReadRequestDTO.ReceiverId);

            // Update the unread count for the receiver
            await _chatHub.Clients.User(directMessageMarkAllAsReadRequestDTO.ReceiverId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageResultDTOs);
        }

        /// <summary>
        /// Updates the content of a direct message and notifies both the sender and receiver via SignalR.
        /// </summary>
        /// <param name="directMessageUpdateRequestDTO">The request containing the message ID and updated content.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated direct message details.
        /// </returns>
        [HttpPatch("update")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDirectMessageAsync([FromBody] DirectMessageUpdateRequestDTO directMessageUpdateRequestDTO)
        {
            DirectMessageUpdateResponseDTO directMessageUpdateResponseDTO = await _directMessageService.UpdateDirectMessageAsync(directMessageUpdateRequestDTO);

            // Notify the receiver about the updated message
            await _chatHub.Clients.User(directMessageUpdateResponseDTO.ReceiverId.ToString())
                .SendAsync("MessageUpdated", directMessageUpdateResponseDTO);

            // Notify the sender about the updated message
            await _chatHub.Clients.User(directMessageUpdateResponseDTO.SenderId.ToString())
                .SendAsync("MessageUpdated", directMessageUpdateResponseDTO);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs_receiver = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageUpdateResponseDTO.ReceiverId);

            // Update the recent message list of the receiver
            await _chatHub.Clients.User(directMessageUpdateResponseDTO.ReceiverId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs_receiver);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs_sender = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageUpdateResponseDTO.SenderId);

            // Update the recent message list of the sender
            await _chatHub.Clients.User(directMessageUpdateResponseDTO.SenderId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs_sender);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageUpdateResponseDTO);
        }

        /// <summary>
        /// Deletes a direct message and notifies both the sender and receiver via SignalR.
        /// </summary>
        /// <param name="messageId">The ID of the message to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the details of the deleted message.
        /// </returns>
        [HttpDelete("delete/{messageId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDirectMessageAsync([FromRoute] long messageId)
        {
            DirectMessageDeleteResponseDTO directMessageDeleteResponseDTO = await _directMessageService.DeleteDirectMessageAsync(messageId);

            // Notify the receiver about the deleted message
            await _chatHub.Clients.User(directMessageDeleteResponseDTO.ReceiverId.ToString())
                .SendAsync("MessageDeleted", directMessageDeleteResponseDTO);

            // Notify the sender about the deleted message
            await _chatHub.Clients.User(directMessageDeleteResponseDTO.SenderId.ToString())
                .SendAsync("MessageDeleted", directMessageDeleteResponseDTO);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs_receiver = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageDeleteResponseDTO.ReceiverId);

            // Update the recent message list of the receiver
            await _chatHub.Clients.User(directMessageDeleteResponseDTO.ReceiverId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs_receiver);

            List<DirectMessagesRecentMessagesResponseDTO>? directMessagesRecentMessagesResponseDTOs = await _directMessageService.GetRecentDirectMessagesBetweenUsersAsync(directMessageDeleteResponseDTO.SenderId);

            // Update the recent message list of the sender
            await _chatHub.Clients.User(directMessageDeleteResponseDTO.SenderId.ToString())
                .SendAsync("UpdateRecentDirectMessages", directMessagesRecentMessagesResponseDTOs);

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageDeleteResponseDTO);
        }

        /// <summary>
        /// Updates a direct message attachment.
        /// </summary>
        /// <param name="directMessageFileAttachmentUpdateRequestDTO">The attachment update request data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the updated attachment details.
        /// </returns>
        [HttpPatch("attachment")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAttachmentAsync([FromForm] DirectMessageFileAttachmentUpdateRequestDTO directMessageFileAttachmentUpdateRequestDTO)
        {
            DirectMessageUpdateFileAttachmentResponseDTO directMessageUpdateFileAttachmentResponseDTO = await _directMessageService.UpdateDirectMessageAttachmentAsync(directMessageFileAttachmentUpdateRequestDTO);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageUpdateFileAttachmentResponseDTO);
        }
        
        /// <summary>
        /// Deletes an attachment from a direct message.
        /// </summary>
        /// <param name="directMessageId">The ID of the direct message whose attachment is to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the result of the deletion.
        /// </returns>
        [HttpDelete("attachment/{directMessageId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDirectMessageAttachmentAsync([FromRoute] int directMessageId)
        {
            DirectMessageDeleteAttachmentResponseDTO directMessageDeleteAttachmentResponseDTO = await _directMessageService.DeleteDirectMessageAttachmentAsync(directMessageId);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, directMessageDeleteAttachmentResponseDTO);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user's details.
        /// </returns>
        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] int id)
        {
            UsersResponseDTO? usersResponseDTO = await _directMessageService.GetUserByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, usersResponseDTO);
        }

        /// <summary>
        /// Searches for users by a keyword in their name, excluding the current user.
        /// </summary>
        /// <param name="keyword">The search keyword.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of matching users.
        /// </returns>
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsersByKeyWordAsync([FromQuery] string? keyword)
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

            List<UsersResponseDTO>? usersResponseDTOs = await _directMessageService.SearchUsersByKeyWordAsync(userId, keyword);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, usersResponseDTOs);
        }

    }
}