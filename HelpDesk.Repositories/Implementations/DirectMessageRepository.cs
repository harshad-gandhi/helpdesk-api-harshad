using System.Data;
using Dapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using static HelpDesk.Common.Enums.Enumerations;

namespace HelpDesk.Repositories.Implementations
{
    public class DirectMessageRepository(IDbConnectionFactory connectionFactory, IStringLocalizer<Messages> localizer) : IDirectMessageRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        private readonly IStringLocalizer<Messages> _localizer = localizer;


        /// <summary>
        /// Sends a direct message by saving it using a stored procedure.
        /// </summary>
        /// <param name="directMessageSendRequestDTO">
        /// The message payload including sender ID, receiver ID, message text, and message type.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageSendResultDTO"/> containing the result of the save operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageSendResultDTO> SendDirectMessageAsync(DirectMessageSendRequestDTO directMessageSendRequestDTO)
        {
            const string spName = "usp_direct_messages_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.ADD_UPDATE);
            parameters.Add("@SenderId", directMessageSendRequestDTO.SenderId);
            parameters.Add("@ReceiverId", directMessageSendRequestDTO.ReceiverId);
            parameters.Add("@DirectMessage", directMessageSendRequestDTO.DirectMessage);
            parameters.Add("@MessageType", directMessageSendRequestDTO.MessageType);

            DirectMessageSendResultDTO directMessageSendResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageSendResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageSendResultDTO;
        }

        /// <summary>
        /// Retrieves direct messages between two users using stored procedures.
        /// </summary>
        /// <param name="directMessagesGetRequestDTO">
        /// The request DTO containing sender ID, receiver ID, optional message ID, pagination, 
        /// sorting, and keyword filter information.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="DirectMessageGetResultDTO"/> containing the messages 
        /// matching the specified criteria.
        /// </returns>
        public async Task<List<DirectMessageGetResultDTO>> GetDirectMessagesBetweenUsersAsync(DirectMessagesGetRequestDTO directMessagesGetRequestDTO)
        {

            if (directMessagesGetRequestDTO.MessageId > 0)
            {
                const string procedure = "usp_direct_messages_search";

                DynamicParameters parameters = new();

                parameters.Add("@SenderId", directMessagesGetRequestDTO.SenderId);
                parameters.Add("@ReceiverId", directMessagesGetRequestDTO.ReceiverId);
                parameters.Add("@MessageId", directMessagesGetRequestDTO.MessageId);
                parameters.Add("@Window", 10);

                List<DirectMessageGetResultDTO>? directMessageGetResultDTOs = (await _baseRepository.QueryAsync<DirectMessageGetResultDTO>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure)).AsList();

                return directMessageGetResultDTOs;
            }
            else
            {
                const string procedure = "usp_direct_messages_get_pagination_by_id";

                DynamicParameters parameters = new();

                parameters.Add("@SenderId", directMessagesGetRequestDTO.SenderId);
                parameters.Add("@ReceiverId", directMessagesGetRequestDTO.ReceiverId);
                // parameters.Add("@Offset", directMessagesGetRequestDTO.Offset);
                parameters.Add("@PageSize", directMessagesGetRequestDTO.PageSize);
                parameters.Add("@LastMessageId", directMessagesGetRequestDTO.LastMessageId);
                parameters.Add("@Is_Direction_Ascending", directMessagesGetRequestDTO.Is_Direction_Ascending);
                if (!string.IsNullOrEmpty(directMessagesGetRequestDTO.KeyWord))
                {
                    parameters.Add("@Keyword", directMessagesGetRequestDTO.KeyWord);
                }

                List<DirectMessageGetResultDTO>? directMessageGetResultDTOs = (await _baseRepository.QueryAsync<DirectMessageGetResultDTO>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure)).AsList();

                return directMessageGetResultDTOs;
            }
        }

        /// <summary>
        /// Retrieves the recent direct messages of the specified sender.
        /// </summary>
        /// <param name="senderId">
        /// The ID of the user whose recent messages are being retrieved.
        /// </param>
        /// <returns>
        /// A sequence of <see cref="DirectMessageRecentResultDTO"/> containing the sender's recent messages.
        /// </returns>
        public async Task<List<DirectMessageRecentResultDTO>> GetRecentDirectMessagesBetweenUsersAsync(int senderId)
        {
            const string procedure = "usp_direct_messages_recent";

            DynamicParameters parameters = new();

            parameters.Add("@SenderId", senderId);

            List<DirectMessageRecentResultDTO>? directMessageRecentResultDTOs = (await _baseRepository.QueryAsync<DirectMessageRecentResultDTO>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure)).AsList();

            return directMessageRecentResultDTOs;
        }

        /// <summary>
        /// Marks a direct message as read in the database using a stored procedure.
        /// </summary>
        /// <param name="messageId">
        /// The unique identifier of the direct message to be marked as read.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageResultDTO"/> containing the result of the operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageResultDTO> MarkDirectMessageAsReadAsync(long messageId)
        {
            const string procedure = "usp_direct_messages_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.READ);
            parameters.Add("@Id", messageId);
            parameters.Add("@MessageReadAt", DateTimeOffset.Now);

            DirectMessageResultDTO directMessageResultDTO =
                    await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageResultDTO>(procedure, parameters, CommandType.StoredProcedure)
                    ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageResultDTO;
        }

        /// <summary>
        /// Marks all direct messages between the specified sender and receiver as read using a stored procedure.
        /// </summary>
        /// <param name="directMessageMarkAllAsReadRequestDTO">
        /// The request containing the sender and receiver IDs for which all messages should be marked as read.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageResultDTO"/> containing the result of the operation.
        /// </returns>
        public async Task<List<DirectMessageResultDTO>> MarkAllDirectMessageAsReadAsync(DirectMessageMarkAllAsReadRequestDTO directMessageMarkAllAsReadRequestDTO)
        {
            const string procedure = "usp_direct_messages_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.READ);
            parameters.Add("@SenderId", directMessageMarkAllAsReadRequestDTO.SenderId);
            parameters.Add("@ReceiverId", directMessageMarkAllAsReadRequestDTO.ReceiverId);
            parameters.Add("@MessageReadAt", DateTimeOffset.Now);

            List<DirectMessageResultDTO>? directMessageResultDTOs = (await _baseRepository.QueryAsync<DirectMessageResultDTO>(
                procedure, parameters, CommandType.StoredProcedure)).AsList();
            return directMessageResultDTOs;
        }

        /// <summary>
        /// Updates a direct message with the specified message ID using a stored procedure.
        /// </summary>
        /// <param name="directMessageUpdateRequestDTO">
        /// The request containing the message ID and new message content to update.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageResultDTO"/> representing the result of the update operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageResultDTO> UpdateDirectMessageAsync(DirectMessageUpdateRequestDTO directMessageUpdateRequestDTO)
        {
            const string procedure = "usp_direct_messages_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.ADD_UPDATE);
            parameters.Add("@Id", directMessageUpdateRequestDTO.MessageId);
            parameters.Add("@DirectMessage", directMessageUpdateRequestDTO.Message);

            DirectMessageResultDTO directMessageResultDTO =
                    await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageResultDTO>(procedure, parameters, CommandType.StoredProcedure)
                    ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageResultDTO;
        }

        /// <summary>
        /// Deletes a direct message with the specified message ID using a stored procedure.
        /// </summary>
        /// <param name="messageId">
        /// The ID of the direct message to delete.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageResultDTO"/> representing the result of the delete operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageResultDTO> DeleteDirectMessageAsync(long messageId)
        {

            const string procedure = "usp_direct_messages_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.DELETE);
            parameters.Add("@Id", messageId);
            parameters.Add("@IsDeleted", true);

            DirectMessageResultDTO directMessageResultDTO =
                    await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageResultDTO>(procedure, parameters, CommandType.StoredProcedure)
                    ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageResultDTO;
        }

        /// <summary>
        /// Retrieves the attachment associated with a specific direct message.
        /// </summary>
        /// <param name="directMessageId">
        /// The ID of the direct message whose attachment is to be retrieved.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageAttachmentResultDTO"/> containing the attachment details, or null if no attachment exists.
        /// </returns>
        public async Task<DirectMessageAttachmentResultDTO?> GetDirectMessageAttachmentAsync(int directMessageId)
        {
            const string procedure = "usp_direct_messages_attachments_get";

            DynamicParameters parameters = new();

            parameters.Add("@DirectMessageId", directMessageId);

            DirectMessageAttachmentResultDTO? directMessageAttachmentResultDTO = await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageAttachmentResultDTO>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            return directMessageAttachmentResultDTO;
        }

        /// <summary>
        /// Saves a file attachment for a direct message in the database.
        /// </summary>
        /// <param name="directMessageAttachmentResultDTO">
        /// The attachment details including the direct message ID, original file name, stored file name, file path, MIME type, and file size.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageSendResultDTO"/> containing the result of the attachment save operation.
        /// </returns>        
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageSendResultDTO> SaveDirectMessageAttachmentAsync(DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO)
        {
            const string procedure = "usp_direct_messages_attachments_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.ADD_UPDATE);
            parameters.Add("@DirectMessageId", directMessageAttachmentResultDTO.DirectMessageId);
            parameters.Add("@OriginalFileName", directMessageAttachmentResultDTO.OriginalFileName);
            parameters.Add("@FileName", directMessageAttachmentResultDTO.FileName);
            parameters.Add("@FilePath", directMessageAttachmentResultDTO.FilePath);
            parameters.Add("@MimeType", directMessageAttachmentResultDTO.MimeType);
            parameters.Add("@FileSizeByte", directMessageAttachmentResultDTO.FileSizeByte);

            DirectMessageSendResultDTO directMessageSendResultDTO = await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageSendResultDTO>(procedure, parameters, CommandType.StoredProcedure) ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageSendResultDTO;
        }

        /// <summary>
        /// Updates an existing file attachment for a direct message in the database.
        /// </summary>
        /// <param name="directMessageAttachmentResultDTO">
        /// The attachment details including its ID, direct message ID, original file name, stored file name, file path, MIME type, and file size.
        /// </param>
        /// <returns>
        /// Returns a <see cref="DirectMessageResultDTO"/> containing the result of the update operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageResultDTO> UpdateDirectMessageAttachmentAsync(DirectMessageAttachmentResultDTO directMessageAttachmentResultDTO)
        {
            const string procedure = "usp_direct_messages_attachments_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.ADD_UPDATE);
            parameters.Add("@Id", directMessageAttachmentResultDTO.Id);
            parameters.Add("@DirectMessageId", directMessageAttachmentResultDTO.DirectMessageId);
            parameters.Add("@OriginalFileName", directMessageAttachmentResultDTO.OriginalFileName);
            parameters.Add("@FileName", directMessageAttachmentResultDTO.FileName);
            parameters.Add("@FilePath", directMessageAttachmentResultDTO.FilePath);
            parameters.Add("@MimeType", directMessageAttachmentResultDTO.MimeType);
            parameters.Add("@FileSizeByte", directMessageAttachmentResultDTO.FileSizeByte);

            DirectMessageResultDTO directMessageResultDTO = await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageResultDTO>(procedure, parameters, CommandType.StoredProcedure) ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageResultDTO;
        }

        /// <summary>
        /// Deletes the file attachment associated with a specific direct message.
        /// </summary>
        /// <param name="directMessageId">The ID of the direct message whose attachment is to be deleted.</param>
        /// <returns>
        /// Returns a <see cref="DirectMessageDeleteAttachmentResultDTO"/> containing the result of the deletion operation.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<DirectMessageDeleteAttachmentResultDTO> DeleteDirectMessageAttachmentAsync(int directMessageId)
        {
            const string procedure = "usp_direct_messages_attachments_save";

            DynamicParameters parameters = new();

            parameters.Add("@ActionType", (int)StoreProcedureActionType.DELETE);
            parameters.Add("@DirectMessageId", directMessageId);
            parameters.Add("@IsDeleted", true);

            DirectMessageDeleteAttachmentResultDTO directMessageDeleteAttachmentResultDTO = await _baseRepository.QueryFirstOrDefaultAsync<DirectMessageDeleteAttachmentResultDTO>(procedure, parameters, CommandType.StoredProcedure) ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return directMessageDeleteAttachmentResultDTO;
        }

        /// <summary>
        /// Retrieves a user's details by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>
        /// Returns a <see cref="UsersResponseDTO"/> containing the user's details.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown if the stored procedure fails or returns null.
        /// </exception>
        public async Task<UsersResponseDTO?> GetUserByIdAsync(int userId)
        {
            string sql = "usp_direct_messages_user_get";

            DynamicParameters parameters = new();

            parameters.Add("@Id", userId);

            UsersResponseDTO usersResponseDTO = await _baseRepository.QueryFirstOrDefaultAsync<UsersResponseDTO>(sql, parameters, commandType: CommandType.StoredProcedure) ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);

            return usersResponseDTO;
        }

        /// <summary>
        /// Searches for users by their keyword.
        /// </summary>
        /// <param name="userId">The ID of the user performing the search.</param>
        /// <param name="keyword">The search keyword to match user names.</param>
        /// <returns>
        /// Returns an <see cref="UsersResponseDTO"/> containing users that match the search criteria.
        /// </returns>
        public async Task<List<UsersResponseDTO>> SearchUsersByKeyWordAsync(int userId, string keyword)
        {
            string sql = "usp_direct_messages_user_get";

            DynamicParameters parameters = new();

            parameters.Add("@Id", userId);
            parameters.Add("@keyword", keyword);

            List<UsersResponseDTO> usersResponseDTOs = (await _baseRepository.QueryAsync<UsersResponseDTO>(sql, parameters, commandType: CommandType.StoredProcedure)).AsList();

            return usersResponseDTOs;
        }

        /// <summary>
        /// Retrieves all countries from the database.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="List{CountryDTO}"/> containing all countries.
        /// </returns>
        public async Task<List<CountryDTO>> GetAllCountriesAsync()
        {
            const string query = "usp_countries_get";

            List<CountryDTO> countryDTOs = (await _baseRepository.QueryAsync<CountryDTO>(
                query,
                null,
                commandType: CommandType.StoredProcedure
            )).AsList();

            return countryDTOs;
        }

        /// <summary>
        /// Searches for countries that match the specified keyword.
        /// </summary>
        /// <param name="keyword">The search keyword used to filter countries.</param>
        /// <returns>
        /// Returns an <see cref="List{CountryDTO}"/> containing countries that match the keyword.
        /// </returns>
        public async Task<IEnumerable<CountryDTO>> SearchCountriesAsync(string keyword)
        {
            string sql = "usp_countries_get";

            DynamicParameters parameters = new();

            parameters.Add("@KeyWord", keyword);

            List<CountryDTO> countryDTOs = (await _baseRepository.QueryAsync<CountryDTO>(
                sql, parameters, commandType: CommandType.StoredProcedure)).AsList();

            return countryDTOs;
        }


        public async Task<CountryDTO?> GetCountryByCodeAsync(string countryCode)
        {
            const string spName = "usp_get_country_by_code";
            DynamicParameters parameters = new();

            parameters.Add("@CountryCode", countryCode);
            CountryDTO? countryDTO = await _baseRepository.QueryFirstOrDefaultAsync<CountryDTO>(spName, parameters, commandType: CommandType.StoredProcedure);
            return countryDTO;
        }

    }
}