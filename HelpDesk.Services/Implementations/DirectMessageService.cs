using AutoMapper;
using HelpDesk.Common.Constants;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using static HelpDesk.Common.Enums.Enumerations;
using static HelpDesk.Common.Helpers.Helper;

namespace HelpDesk.Services.Implementations
{
    public class DirectMessageService(IDirectMessageRepository directMessageRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IDirectMessageService
    {
        private readonly IDirectMessageRepository _directMessageRepository = directMessageRepository;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Sends a direct message from one user to another, optionally with a file attachment, and returns the result including file path if applicable.
        /// </summary>
        /// <param name="directMessageSendRequestDTO">The request containing sender, receiver, message content, and optional file attachment.</param>
        /// <returns>A <see cref="DirectMessageSendResponseDTO"/> containing the message details and file path if uploaded.</returns>
        /// <exception cref="BadRequestException">Thrown when the sender tries to send a message to themselves.</exception>
        /// <exception cref="ValidationException">Thrown when the attached file type is invalid.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure saving the message or attachment, or if the result code from the repository indicates an error.
        /// </exception>
        public async Task<DirectMessageSendResponseDTO> SendDirectMessageAsync(DirectMessageSendRequestDTO directMessageSendRequestDTO)
        {
            // Block Self Message
            if (directMessageSendRequestDTO.SenderId == directMessageSendRequestDTO.ReceiverId)
                throw new BadRequestException(_localizer["INVALID_ACTION"]);

            // Get Message Type
            if (directMessageSendRequestDTO.File != null)
            {
                string contentType = directMessageSendRequestDTO.File.ContentType.ToLowerInvariant();

                if (!FileConstants.AllowedFileTypes.Contains(contentType))
                {
                    throw new ValidationException(_localizer["INVALID_FILE_TYPE"]);
                }

                directMessageSendRequestDTO.MessageType = contentType switch
                {
                    SystemConstant.JPEG or SystemConstant.PNG => (int)DirectMessageType.Image,
                    SystemConstant.PDF or SystemConstant.TEXT_PLAN => (int)DirectMessageType.File,
                    _ => (int)DirectMessageType.Text
                };
            }
            else
            {
                directMessageSendRequestDTO.MessageType = (int)DirectMessageType.Text;
            }

            // Save direct message
            DirectMessageSendResultDTO directMessageSendResultDTO = await _directMessageRepository.SendDirectMessageAsync(directMessageSendRequestDTO);

            DirectMessageSendResponseDTO directMessageSendResponseDTO = _mapper.Map<DirectMessageSendResponseDTO>(directMessageSendResultDTO);

            // Check for the success
            switch (directMessageSendResultDTO.ResultCode)
            {
                case (int)ResultCode.InsertSuccess:
                    directMessageSendResponseDTO.Message = _localizer["SEND_SUCCESS", _localizer["DIRECT_MESSAGE"]];
                    break;

                case (int)ResultCode.InvalidAction:
                    throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                default:
                    if (directMessageSendResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }


            // Save attachment if present 
            if (directMessageSendRequestDTO.File != null)
            {
                DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO = await SaveFileInLocalAsync(directMessageSendRequestDTO.File, directMessageSendResultDTO.AffectedId);

                DirectMessageSendResultDTO directMessageSendResultDTO_attachment = await _directMessageRepository.SaveDirectMessageAttachmentAsync(directMessageAttachmentResultDTO);

                if (!string.IsNullOrWhiteSpace(directMessageSendResultDTO_attachment.FilePath))
                {
                    string baseUrl = SystemConstant.API_BASE_URL;
                    directMessageSendResponseDTO.FilePath = $"{baseUrl}/{directMessageSendResultDTO_attachment.FilePath
                        .Replace(SystemConstant.ROOT_FOLDER_Path, "")
                        .Replace("\\", "/")}";
                    directMessageSendResponseDTO.OriginalFileName = directMessageAttachmentResultDTO.OriginalFileName;
                }


                switch (directMessageSendResultDTO.ResultCode)
                {
                    case (int)ResultCode.InsertSuccess:
                        directMessageSendResponseDTO.Message = _localizer["SEND_SUCCESS", _localizer["DIRECT_MESSAGE_ATTACHMENT"]];
                        break;

                    case (int)ResultCode.InvalidAction:
                        throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                    default:
                        if (directMessageSendResultDTO.ResultCode <= 0)
                            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }

            }

            // Return file path if present
            if (directMessageSendRequestDTO.MessageType != 1 && !string.IsNullOrWhiteSpace(directMessageSendResultDTO.FilePath))
            {
                string baseUrl = SystemConstant.API_BASE_URL;
                directMessageSendResponseDTO.FilePath = $"{baseUrl}/{directMessageSendResultDTO.FilePath.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "/")}" ??
                throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
            }

            return directMessageSendResponseDTO;
        }

        /// <summary>
        /// Retrieves all direct messages between the sender and receiver, including file paths if messages contain attachments.
        /// </summary>
        /// <param name="directMessagesGetRequestDTO">The request containing sender and receiver IDs and optional filters.</param>
        /// <returns>An enumerable of <see cref="DirectMessageGetResponseDTO"/> representing the messages exchanged.</returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an error while resolving the file path for attached files.
        /// </exception>
        public async Task<List<DirectMessageGetResponseDTO>?> GetDirectMessagesBetweenUsersAsync(DirectMessagesGetRequestDTO directMessagesGetRequestDTO)
        {
            List<DirectMessageGetResultDTO>? directMessageGetResultDTOs = await _directMessageRepository.GetDirectMessagesBetweenUsersAsync(directMessagesGetRequestDTO);

            List<DirectMessageGetResponseDTO> directMessageGetResponseDTOs = _mapper.Map<List<DirectMessageGetResponseDTO>>(directMessageGetResultDTOs);

            if (directMessageGetResponseDTOs != null)
            {
                foreach (DirectMessageGetResponseDTO directMessageGetResponseDTO in directMessageGetResponseDTOs)
                {
                    if (directMessageGetResponseDTO.MessageType != 1 && !string.IsNullOrWhiteSpace(directMessageGetResponseDTO.FilePath))
                    {
                        string baseUrl = SystemConstant.API_BASE_URL;
                        directMessageGetResponseDTO.FilePath = $"{baseUrl}/{directMessageGetResponseDTO.FilePath.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "/")}" ??
                        throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
                    }
                }
            }
            return directMessageGetResponseDTOs;
        }

        /// <summary>
        /// Retrieves the most recent direct messages for a given sender, including avatar URLs for each user.
        /// </summary>
        /// <param name="senderId">The ID of the sender whose recent messages are being retrieved.</param>
        /// <returns>An enumerable of <see cref="DirectMessagesRecentMessagesResponseDTO"/> representing the recent messages.</returns>
        /// <exception cref="BadRequestException">Thrown when the provided sender ID is less than or equal to zero.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an error while resolving the avatar file path.
        /// </exception>
        public async Task<List<DirectMessagesRecentMessagesResponseDTO>?> GetRecentDirectMessagesBetweenUsersAsync(int senderId)
        {
            if (senderId <= 0)
                throw new BadRequestException(_localizer["ID_POSITIVE", "ID"]);

            List<DirectMessageRecentResultDTO>? directMessageRecentResultDTOs = await _directMessageRepository.GetRecentDirectMessagesBetweenUsersAsync(senderId);

            List<DirectMessagesRecentMessagesResponseDTO> directMessagesRecentMessagesResponseDTOs = _mapper.Map<List<DirectMessagesRecentMessagesResponseDTO>>(directMessageRecentResultDTOs);

            foreach (DirectMessagesRecentMessagesResponseDTO directMessagesRecentMessagesResponseDTO in directMessagesRecentMessagesResponseDTOs)
            {
                if (!string.IsNullOrWhiteSpace(directMessagesRecentMessagesResponseDTO.AvatarUrl))
                {
                    string baseUrl = SystemConstant.API_BASE_URL;
                    directMessagesRecentMessagesResponseDTO.AvatarUrl = $"{baseUrl}/uploads/{directMessagesRecentMessagesResponseDTO.AvatarUrl.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "")}" ??
                    throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
                }
            }

            return directMessagesRecentMessagesResponseDTOs;
        }

        /// <summary>
        /// Marks a specific direct message as read and returns the updated message status.
        /// </summary>
        /// <param name="messageId">The ID of the message to mark as read.</param>
        /// <returns>A <see cref="DirectMessageMarkAsReadResponseDTO"/> containing the updated read status.</returns>
        /// <exception cref="BadHttpRequestException">Thrown when the provided message ID is less than or equal to zero.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the action is invalid or an internal error occurs during the update.
        /// </exception>
        public async Task<DirectMessageMarkAsReadResponseDTO> MarkDirectMessageAsReadAsync(long messageId)
        {
            if (messageId <= 0)
                throw new BadHttpRequestException(_localizer["ID_POSITIVE", "ID"]);

            DirectMessageResultDTO directMessageResultDTO = await _directMessageRepository.MarkDirectMessageAsReadAsync(messageId);

            DirectMessageMarkAsReadResponseDTO directMessageMarkAsReadResponseDTO = _mapper.Map<DirectMessageMarkAsReadResponseDTO>(directMessageResultDTO);
            switch (directMessageResultDTO.ResultCode)
            {

                case (int)ResultCode.ReadSuccess:
                    directMessageMarkAsReadResponseDTO.Message = _localizer["READ_SUCCESS", _localizer["DIRECT_MESSAGE"]];
                    break;

                case (int)ResultCode.InvalidAction:
                    throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                default:
                    if (directMessageResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }
            return directMessageMarkAsReadResponseDTO;
        }

        /// <summary>
        /// Marks all direct messages as read for the specified receiver and returns the results.
        /// </summary>
        /// <param name="directMessageMarkAllAsReadRequestDTO">The request DTO containing receiver information.</param>
        /// <returns>A list of <see cref="DirectMessageResultDTO"/> representing the updated messages.</returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when no messages are updated or if an invalid action or internal error occurs during the update.
        /// </exception>
        public async Task<List<DirectMessageResultDTO>> MarkAllDirectMessageAsReadAsync(DirectMessageMarkAllAsReadRequestDTO directMessageMarkAllAsReadRequestDTO)
        {


            List<DirectMessageResultDTO> directMessageResultDTOs = await _directMessageRepository.MarkAllDirectMessageAsReadAsync(directMessageMarkAllAsReadRequestDTO);

            if (directMessageResultDTOs == null || directMessageResultDTOs.Count == 0)
            {
                throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }

            foreach (DirectMessageResultDTO directMessageResultDTO in directMessageResultDTOs)
            {
                switch (directMessageResultDTO.ResultCode)
                {
                    case (int)ResultCode.ReadSuccess:
                        break;

                    case (int)ResultCode.InvalidAction:
                        throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                    default:
                        if (directMessageResultDTO.ResultCode <= 0)
                            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            return directMessageResultDTOs;
        }

        /// <summary>
        /// Updates a direct message and returns the updated message details.
        /// </summary>
        /// <param name="directMessageUpdateRequestDTO">The request DTO containing message update details.</param>
        /// <returns>A <see cref="DirectMessageUpdateResponseDTO"/> representing the updated message.</returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the update fails due to an invalid action or internal server error.
        /// </exception>
        public async Task<DirectMessageUpdateResponseDTO> UpdateDirectMessageAsync(DirectMessageUpdateRequestDTO directMessageUpdateRequestDTO)
        {
            DirectMessageResultDTO directMessageResultDTO = await _directMessageRepository.UpdateDirectMessageAsync(directMessageUpdateRequestDTO);

            DirectMessageUpdateResponseDTO directMessageUpdateResponseDTO = _mapper.Map<DirectMessageUpdateResponseDTO>(directMessageResultDTO);
            switch (directMessageResultDTO.ResultCode)
            {

                case (int)ResultCode.UpdateSuccess:
                    directMessageUpdateResponseDTO.Message = _localizer["UPDATED_SUCCESS", _localizer["DIRECT_MESSAGE"]];
                    break;

                case (int)ResultCode.InvalidAction:
                    throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                default:
                    if (directMessageResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }
            return directMessageUpdateResponseDTO;
        }

        /// <summary>
        /// Deletes a direct message by its ID and returns the deletion result.
        /// </summary>
        /// <param name="messageId">The ID of the direct message to delete.</param>
        /// <returns>A <see cref="DirectMessageDeleteResponseDTO"/> representing the deletion result.</returns>
        /// <exception cref="BadRequestException">Thrown when the provided message ID is not positive.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the deletion fails due to an invalid action or internal server error.
        /// </exception>
        public async Task<DirectMessageDeleteResponseDTO> DeleteDirectMessageAsync(long messageId)
        {
            if (messageId <= 0)
                throw new BadRequestException(_localizer["ID_POSITIVE", "DIRECT_MESSAGE"]);

            DirectMessageResultDTO directMessageResultDTO = await _directMessageRepository.DeleteDirectMessageAsync(messageId);

            DirectMessageDeleteResponseDTO directMessageDeleteResponseDTO = _mapper.Map<DirectMessageDeleteResponseDTO>(directMessageResultDTO);

            switch (directMessageResultDTO.ResultCode)
            {
                case (int)ResultCode.DeleteSuccess:
                    directMessageDeleteResponseDTO.Message = _localizer["DELETE_SUCCESS"];
                    break;

                case (int)ResultCode.InvalidAction:
                    throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                default:
                    if (directMessageResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }
            return directMessageDeleteResponseDTO;
        }

        /// <summary>
        /// Updates the file attachment of a direct message.
        /// Deletes the existing file if present and saves the new file.
        /// </summary>
        /// <param name="directMessageFileAttachmentUpdateRequestDTO">The request DTO containing the direct message ID and the new file.</param>
        /// <returns>A <see cref="DirectMessageUpdateFileAttachmentResponseDTO"/> representing the updated attachment.</returns>
        /// <exception cref="BadRequestException">Thrown when the new file is not provided.</exception>
        /// <exception cref="NotFoundException">Thrown when the existing attachment is not found.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the update fails due to an invalid action or internal server error.
        /// </exception>
        public async Task<DirectMessageUpdateFileAttachmentResponseDTO> UpdateDirectMessageAttachmentAsync(DirectMessageFileAttachmentUpdateRequestDTO directMessageFileAttachmentUpdateRequestDTO)
        {
            if (directMessageFileAttachmentUpdateRequestDTO.NewFile == null)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "File"));

            DirectMessageAttachmentResultDTO existingAttachment = await _directMessageRepository.GetDirectMessageAttachmentAsync(directMessageFileAttachmentUpdateRequestDTO.DirectMessageId) ?? throw new NotFoundException(string.Format(_localizer["DATA_NOT_FOUND", SystemConstant.DIRECT_MESSAGE_ATTACHMENT]));

            // Delete old file if exists
            if (!string.IsNullOrEmpty(existingAttachment.FilePath) && File.Exists(existingAttachment.FilePath))
            {
                File.Delete(existingAttachment.FilePath);
            }

            DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO = await SaveFileInLocalAsync(directMessageFileAttachmentUpdateRequestDTO.NewFile, directMessageFileAttachmentUpdateRequestDTO.DirectMessageId, existingAttachment.Id);

            DirectMessageResultDTO directMessageResultDTO = await _directMessageRepository.UpdateDirectMessageAttachmentAsync(directMessageAttachmentResultDTO);

            DirectMessageUpdateFileAttachmentResponseDTO directMessageUpdateFileAttachmentResponseDTO = _mapper.Map<DirectMessageUpdateFileAttachmentResponseDTO>(directMessageResultDTO);

            switch (directMessageResultDTO.ResultCode)
            {
                case (int)ResultCode.UpdateSuccess:
                    directMessageUpdateFileAttachmentResponseDTO.Message = _localizer["UPDATED_SUCCESS", _localizer["DIRECT_MESSAGE_ATTACHMENT"]];
                    break;

                case (int)ResultCode.InvalidAction:
                    throw new InternalServerErrorException(_localizer["INVALID_ACTION"]);

                default:
                    if (directMessageResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }

            return directMessageUpdateFileAttachmentResponseDTO;

        }

        /// <summary>
        /// Deletes the file attachment of a direct message by its ID.
        /// </summary>
        /// <param name="directMessageId">The ID of the direct message whose attachment is to be deleted.</param>
        /// <returns>A <see cref="DirectMessageDeleteAttachmentResponseDTO"/> representing the deletion result.</returns>
        /// <exception cref="BadRequestException">Thrown when the direct message ID is not positive.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the deletion fails due to an internal server error.
        /// </exception>
        public async Task<DirectMessageDeleteAttachmentResponseDTO> DeleteDirectMessageAttachmentAsync(int directMessageId)
        {
            if (directMessageId <= 0)
                throw new BadRequestException(_localizer["ID_POSITIVE", "DIRECT_MESSAGE"]);

            DirectMessageDeleteAttachmentResultDTO directMessageDeleteAttachmentResultDTO = await _directMessageRepository.DeleteDirectMessageAttachmentAsync(directMessageId);

            DirectMessageDeleteAttachmentResponseDTO directMessageDeleteAttachmentResponseDTO = _mapper.Map<DirectMessageDeleteAttachmentResponseDTO>(directMessageDeleteAttachmentResultDTO);

            switch (directMessageDeleteAttachmentResultDTO.ResultCode)
            {
                case (int)ResultCode.DeleteSuccess:
                    directMessageDeleteAttachmentResponseDTO.Message = _localizer["DELETE_SUCCESS"];
                    break;

                default:
                    if (directMessageDeleteAttachmentResultDTO.ResultCode <= 0)
                        throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);

                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }
            return directMessageDeleteAttachmentResponseDTO;
        }

        /// <summary>
        /// Retrieves a user's details by their ID, including the avatar URL if present.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>A <see cref="UsersResponseDTO"/> containing user details, or null if not found.</returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if there is an error while processing the user's avatar file path.
        /// </exception>
        public async Task<UsersResponseDTO?> GetUserByIdAsync(int userId)
        {
            UsersResponseDTO? usersResponseDTO = await _directMessageRepository.GetUserByIdAsync(userId);

            if (usersResponseDTO != null && !string.IsNullOrWhiteSpace(usersResponseDTO.AvatarUrl))
            {
                string baseUrl = SystemConstant.API_BASE_URL;
                usersResponseDTO.AvatarUrl = $"{baseUrl}/{usersResponseDTO.AvatarUrl.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "")}" ??
                throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
            }
            return usersResponseDTO;
        }

        /// <summary>
        /// Searches for users by name using the specified keyword, returning their details including avatar URLs if present.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="keyword">The keyword to search users by.</param>
        /// <returns>A collection of <see cref="UsersResponseDTO"/> matching the search criteria.</returns>
        /// <exception cref="BadRequestException">
        /// Thrown if the search keyword is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if there is an error while processing any user's avatar file path.
        /// </exception>
        public async Task<List<UsersResponseDTO>> SearchUsersByKeyWordAsync(int userId, string? keyword)
        {
            keyword = keyword?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))

                throw new BadRequestException(string.Format(_localizer["REQUIRED"], SystemConstant.KEYWORD));

            List<UsersResponseDTO> usersResponseDTOs = await _directMessageRepository.SearchUsersByKeyWordAsync(userId, keyword);

            foreach (UsersResponseDTO usersResponseDTO in usersResponseDTOs)
            {
                if (!string.IsNullOrWhiteSpace(usersResponseDTO.AvatarUrl))
                {
                    string baseUrl = SystemConstant.API_BASE_URL;
                    usersResponseDTO.AvatarUrl = $"{baseUrl}/{usersResponseDTO.AvatarUrl.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "")}" ??
                    throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
                }
            }
            return usersResponseDTOs;
        }

        /// <summary>
        /// Retrieves a list of all countries.
        /// </summary>
        /// <returns>A collection of <see cref="CountryDTO"/> representing all countries.</returns>
        public async Task<List<CountryDTO>> GetAllCountriesAsync()
        {
            List<CountryDTO> countryDTOs = await _directMessageRepository.GetAllCountriesAsync();
            return countryDTOs;
        }

        /// <summary>
        /// Searches for countries matching the specified keyword.
        /// </summary>
        /// <param name="keyword">The search keyword.</param>
        /// <returns>A collection of <see cref="CountryDTO"/> matching the keyword.</returns>
        /// <exception cref="BadRequestException">Thrown if the keyword is null, empty, or whitespace.</exception>
        public async Task<IEnumerable<CountryDTO>> SearchCountriesAsync(string? keyword)
        {
            keyword = keyword?.Trim();
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], SystemConstant.KEYWORD));
            IEnumerable<CountryDTO> countryDTOs = await _directMessageRepository.SearchCountriesAsync(keyword);
            return countryDTOs;
        }

        /// <summary>
        /// Saves the given file to a local directory with a unique name and returns its metadata.
        /// </summary>
        /// <param name="file">The file to save.</param>
        /// <param name="directMessageId">The ID of the associated direct message.</param>
        /// <param name="attachmentId">Optional ID of an existing attachment to update.</param>
        /// <returns>A <see cref="DirectMessageAttachmentResultDTO"/> containing the saved file's details.</returns>
        /// <exception cref="InternalServerErrorException">Thrown if there is an error while saving the file.</exception>
        private async Task<DirectMessageAttachmentResultDTO> SaveFileInLocalAsync(IFormFile file, int directMessageId, int? attachmentId = null)
        {
            try 
            {
                string fileExtension = Path.GetExtension(file.FileName);
                string guid = Guid.NewGuid().ToString();
                string folderName = guid[..2];
                string folderPath = Path.Combine(SystemConstant.ROOT_FOLDER, SystemConstant.UPLOADS_FOLDER, folderName);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string uniqueFileName = $"{guid}{fileExtension}";
                string filePath = Path.Combine(folderPath, uniqueFileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return new DirectMessageAttachmentResultDTO
                {
                    Id = attachmentId ?? 0,
                    DirectMessageId = directMessageId,
                    FileName = uniqueFileName,
                    OriginalFileName = file.FileName,
                    FilePath = filePath,
                    MimeType = file.ContentType switch
                    {
                        SystemConstant.JPEG => (int)MimeType.Jpeg,
                        SystemConstant.PNG => (int)MimeType.Png,
                        SystemConstant.PDF => (int)MimeType.Pdf,
                        SystemConstant.TEXT_PLAN => (int)MimeType.TextPlain,
                        SystemConstant.TEXT_HTML => (int)MimeType.TextHtml,
                        _ => (int)MimeType.Unknown
                    },
                    FileSizeByte = (int)file.Length,
                    IsDeleted = false
                };
            }
            catch
            {
                throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
            }
        }

        public async Task<CountryDTO> GetCountryByCodeAsync(string countryCode)
        {
             CountryDTO? countryDTO = await _directMessageRepository.GetCountryByCodeAsync(countryCode);

            if (countryDTO == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_COUNTRY"]));
            }

            return countryDTO;
        }

    }
}