using AutoMapper;
using HelpDesk.Common.Constants;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using static HelpDesk.Common.Enums.Enumerations;
using static HelpDesk.Common.Helpers.Helper;

namespace HelpDesk.Services.Implementations
{
    public class ChatService(IChatSessionsRepository chatSessionsRepository,
        IChatsTransfersRepository chatsTransfersRepository,
        IChatsTagsMappingRepository chatsTagsMappingRepository,
        IChatMessagesRepository chatMessagesRepository,
        IChatAttachmentsRepository chatAttachmentsRepository,
        IMapper mapper, IStringLocalizer<Messages> localizer) : IChatService
    {
        private readonly IChatSessionsRepository _chatSessionRepository = chatSessionsRepository;
        private readonly IChatsTransfersRepository _chatsTransfersRepository = chatsTransfersRepository;
        private readonly IChatsTagsMappingRepository _chatsTagsMappingRepository = chatsTagsMappingRepository;
        private readonly IChatMessagesRepository _chatMessagesRepository = chatMessagesRepository;
        private readonly IChatAttachmentsRepository _chatAttachmentsRepository = chatAttachmentsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<Messages> _localizer = localizer;

        #region ChatSession
        
        /// <summary>
        /// Create Chat Session
        /// </summary>
        /// <param name="chatSessionCreateDto"></param>
        /// <returns>
        /// Returns a tuple containing the created <see cref="ChatSessionDto"/> and a boolean indicating whether the chat session already existed. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<(ChatSessionDto, bool existingchat)> CreateChatSessionAsync(ChatSessionsCreateDto chatSessionCreateDto)
        {
            ChatSessionDto chatSession = _mapper.Map<ChatSessionDto>(chatSessionCreateDto);

            StoredProcedureResult<long> result = await _chatSessionRepository.CreateOrUpdateChatSessionAsync(chatSession);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.PersonHasActiveChat)
                {
                    ChatSessionsListDto existingChatsession = await GetChatSessionByIdAsync(result.Data);
                    ChatSessionDto chatSessionDto = _mapper.Map<ChatSessionDto>(existingChatsession);
                    return (chatSessionDto, true);
                }
            }

            chatSession.Id = result.Data;

            return (chatSession, false);
        }

        /// <summary>
        /// Update Chat Session
        /// </summary>
        /// <param name="chatSessionDto"></param>
        /// <returns>
        /// Returns the updated <see cref="ChatSessionDto"/>. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<ChatSessionDto> UpdateChatSessionAsync(ChatSessionDto chatSessionDto)
        {
            StoredProcedureResult<long> result = await _chatSessionRepository.CreateOrUpdateChatSessionAsync(chatSessionDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            chatSessionDto.Id = result.Data;

            return chatSessionDto;
        }

        /// <summary>
        /// Delete Chat Session
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chat session was successfully deleted. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<bool> DeleteChatSessionAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHAT_SESSION_ID"]));
            }

            StoredProcedureResult<bool> result = await _chatSessionRepository.DeleteChatSessionAsync(id);

            if (!(result.ReturnValue > 0))
            {
                throw result.ReturnValue switch
                {
                    (int)StatusCode.InternalServerError => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
                    (int)StatusCode.ChatSessionDataNotFound => new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHAT_SESSION"])),
                    _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"])
                };
            }

            return true;
        }

        /// <summary>
        /// Get Chat Session By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="ChatSessionsListDto"/> for the specified chat session ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ChatSessionsListDto> GetChatSessionByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHAT_SESSION_ID"]));
            }

            ChatSessionsListDto? chatSession = await _chatSessionRepository.GetChatSessionsByIdAsync(id);

            if (chatSession == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHAT_SESSION"]));
            }

            return chatSession;
        }

        /// <summary>
        /// Get All Chat Sessions By Filter
        /// </summary>
        /// <param name="chatSessionFilterDto"></param>
        /// <returns>
        /// Returns a <see cref="PagedResult{ChatSessionsListDto}"/> containing the filtered chat sessions.
        /// </returns>
        public async Task<PagedResult<ChatSessionsListDto>> GetAllChatSessionsByFilterAsync(ChatSessionsFilterDto chatSessionFilterDto)
        {
            PagedResult<ChatSessionsListDto> chatSessions = await _chatSessionRepository.GetChatSessionsAsync(chatSessionFilterDto);
            return chatSessions;
        }

        /// <summary>
        /// Get Recent Chat Sessions
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="personId"></param>
        /// <returns>
        /// Returns a list of <see cref="ChatSessionsListDto"/> representing recent chat sessions for the specified project and person. 
        /// </returns>
        public async Task<List<ChatSessionsListDto>> GetRecentChatSessionsAsync(long projectId, int personId)
        {
            List<ChatSessionsListDto> chatSessions = await _chatSessionRepository.GetRecentChatSessionsAsync(projectId, personId);
            return chatSessions;
        }

        /// <summary>
        /// Mark Chat Session as Trash
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inTrash"></param>
        /// <param name="updatedAt"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chat session was successfully marked as trash. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<bool> ChatSessionMarkTrashAsync(int id, bool inTrash, DateTimeOffset updatedAt)
        {
            ChatSessionDto? chatSession = new()
            {
                Id = id,
                InTrash = inTrash,
                UpdatedAt = updatedAt,
            };
            StoredProcedureResult<long> result = await _chatSessionRepository.CreateOrUpdateChatSessionAsync(chatSession);
            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }
            return true;

        }

        /// <summary>
        /// Mark Chat Session as Spam
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isSpam"></param>
        /// <param name="updatedAt"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chat session was successfully marked as spam. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<bool> ChatsessionMarkSpamAsync(int id, bool isSpam, DateTimeOffset updatedAt)
        {
            ChatSessionDto? chatSession = new()
            {
                Id = id,
                IsSpam = isSpam,
                UpdatedAt = updatedAt,
            };
            StoredProcedureResult<long> result = await _chatSessionRepository.CreateOrUpdateChatSessionAsync(chatSession);
            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }
            return true;

        }

        /// <summary>
        /// Change Chat Session Status
        /// </summary>
        /// <param name="chatsessionId"></param>
        /// <param name="newStatus"></param>
        /// <returns>
        /// Returns a string message indicating the status change of the chat session. 
        /// </returns>
        public async Task<string> ChangeChatsessionStatusAsync(long chatsessionId, int newStatus)
        {
            ChatSessionsListDto chatsession = await GetChatSessionByIdAsync(chatsessionId);
            int oldStatus = chatsession.ChatSessionStatus;
            string message = $"Chat Session status changed from {(ChatStatus)oldStatus} to {(ChatStatus)newStatus}.";

            ChatMessagesCreateDto chatMessagesCreateDto = new()
            {
                ChatSessionId = chatsessionId,
                MessageType = (int)Enumerations.ChatMessageType.ChatStatusChange,
                SenderId = null,
                SenderType = (int)Enumerations.ChatMessageSenderType.System,
                ChatMessage = message,
                CreatedAt = DateTimeOffset.UtcNow,
                VisibleTo = (int)Enumerations.Visibility.Public
            };
            await CreateChatMessagesAsync(chatMessagesCreateDto);

            ChatSessionDto chatSessionDto = new()
            {
                Id = chatsessionId,
                ChatSessionStatus = newStatus,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            await UpdateChatSessionAsync(chatSessionDto);
            return message;

        }
        #endregion

        #region ChatTransfers

        /// <summary>
        /// Create Chat Transfers
        /// </summary>
        /// <param name="chatTransferCreateDto"></param>
        /// <returns>
        /// Returns the created <see cref="ChatTransfersDto"/>. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<ChatTransfersDto> CreateChatTransfersAsync(ChatTransfersCreateDto chatTransferCreateDto)
        {
            ChatTransfersDto chatTransferDto = _mapper.Map<ChatTransfersDto>(chatTransferCreateDto);

            StoredProcedureResult<int> result = await _chatsTransfersRepository.CreateOrUpdateChatChatTransfersAsync(chatTransferDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }
            chatTransferDto.Id = result.Data;

            return chatTransferDto;
        }

        /// <summary>
        /// Update Chat Transfers
        /// </summary>
        /// <param name="chatTransferDto"></param>
        /// <returns>
        /// Returns the updated <see cref="ChatTransfersDto"/>. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<ChatTransfersDto> UpdateChatTransfersAsync(ChatTransfersDto chatTransferDto)
        {
            StoredProcedureResult<int> result = await _chatsTransfersRepository.CreateOrUpdateChatChatTransfersAsync(chatTransferDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            chatTransferDto.Id = result.Data;
            return chatTransferDto;
        }

        /// <summary>
        ///  Delete Chat Transfers
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chat transfer was successfully deleted. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<bool> DeleteChatTransfersAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_TRANSFERS_ID"]));
            }

            StoredProcedureResult<bool> result = await _chatsTransfersRepository.DeleteChatTransfersAsync(id);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.ChatTransferDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_TRANSFERS"]));
                }
            }
            return true;
        }

        /// <summary>
        /// Get Chat Transfers By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="ChatTransfersDto"/> for the specified chat transfer ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ChatTransfersDto> GetChatTransfersByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_TRANSFERS_ID"]));
            }

            ChatTransfersDto? chatTransfer = await _chatsTransfersRepository.GetChatTransfersByIdAsync(id);

            if (chatTransfer == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_TRANSFERS"]));
            }

            return chatTransfer;
        }

        /// <summary>
        /// Get All Chat Transfers By Chat Session Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns a list of <see cref="ChatTransfersDto"/> for the specified chat session ID. 
        /// </returns>
        public async Task<List<ChatTransfersDto>> GetAllChatTransfersByChatSessionIdAsync(long id)
        {
            List<ChatTransfersDto> chatTransfers = await _chatsTransfersRepository.GetAllChatTransfersByChatSessionIdAsync(id);
            return chatTransfers;
        }
        #endregion

        #region ChatsTagsMapping

        /// <summary>
        /// Create Chats Tags Mapping
        /// </summary>
        /// <param name="chatsTagsMappingCreateDto"></param>
        /// <returns>
        /// Returns the created <see cref="ChatsTagsMappingDto"/>. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="DataAlreadyExistsException"></exception>
        public async Task<ChatsTagsMappingDto> CreateChatsTagsMappingAsync(ChatsTagsMappingCreateDto chatsTagsMappingCreateDto)
        {
            ChatsTagsMappingDto chatsTagsMappingDto = _mapper.Map<ChatsTagsMappingDto>(chatsTagsMappingCreateDto);

            StoredProcedureResult<int> result = await _chatsTagsMappingRepository.CreateOrUpdateChatTagsMappingAsync(chatsTagsMappingDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.ChatsTagsMappingAlreadyExists)
                {
                    throw new DataAlreadyExistsException(string.Format(_localizer["DATA_ALREADY_EXIST"], _localizer["FIELD_CHATS_TAGS_MAPPING"]));
                }
            }
            chatsTagsMappingDto.Id = result.Data;
            return chatsTagsMappingDto;
        }

        /// <summary>
        /// Update Chat Tags Mapping
        /// </summary>
        /// <param name="chatsTagsMappingDto"></param>
        /// <returns>
        /// Returns the updated <see cref="ChatsTagsMappingDto"/>. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="DataAlreadyExistsException"></exception>
        public async Task<ChatsTagsMappingDto> UpdateChatTagsMappingAsync(ChatsTagsMappingDto chatsTagsMappingDto)
        {
            StoredProcedureResult<int> result = await _chatsTagsMappingRepository.CreateOrUpdateChatTagsMappingAsync(chatsTagsMappingDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.ChatsTagsMappingAlreadyExists)
                {
                    throw new DataAlreadyExistsException(string.Format(_localizer["DATA_ALREADY_EXIST"], _localizer["FIELD_CHATS_TAGS_MAPPING"]));
                }
            }

            chatsTagsMappingDto.Id = result.Data;
            return chatsTagsMappingDto;
        }

        /// <summary>
        /// Delete Chats Tags Mapping
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chats tags mapping was successfully deleted. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<bool> DeleteChatsTagsMappingAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_TAGS_MAPPING_ID"]));
            }

            StoredProcedureResult<bool> result = await _chatsTagsMappingRepository.DeleteChatsTagsMappingAsync(id);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.ChatsTagsMappingDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_TAGS_MAPPING"]));
                }
            }
            return true;
        }

        /// <summary>
        /// Get Chats Tags Mapping By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="ChatsTagsMappingDto"/> for the specified chats tags mapping ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ChatsTagsMappingDto> GetChatsTagsMappingByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_TAGS_MAPPING_ID"]));
            }

            ChatsTagsMappingDto? chatsTagsMapping = await _chatsTagsMappingRepository.GetChatsTagsMappingByIdAsync(id);

            if (chatsTagsMapping == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_TAGS_MAPPING"]));
            }
            return chatsTagsMapping;
        }
        #endregion

        #region ChatMessages
        
        /// <summary>
        ///  Create Chat Messages
        /// </summary>
        /// <param name="chatMessagesCreateDto"></param>
        /// <returns>
        /// Returns the created <see cref="ChatMessagesDto"/>. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<ChatMessagesDto> CreateChatMessagesAsync(ChatMessagesCreateDto chatMessagesCreateDto)
        {
            if (chatMessagesCreateDto.File != null)
            {
                string contentType = chatMessagesCreateDto.File.ContentType.ToLowerInvariant();

                if (!FileConstants.AllowedFileTypes.Contains(contentType))
                {
                    throw new ValidationException(_localizer["INVALID_FILE_TYPE"]);
                }
                chatMessagesCreateDto.MessageType = contentType switch
                {
                    SystemConstant.JPEG or SystemConstant.PNG or SystemConstant.PDF or SystemConstant.TEXT_PLAN => (int)ChatMessageType.File,
                    _ => (int)ChatMessageType.Text
                };
            }
            

            ChatMessagesDto chatMessagesDto = _mapper.Map<ChatMessagesDto>(chatMessagesCreateDto);

            StoredProcedureResult<long> result = await _chatMessagesRepository.CreateOrUpdateChatMessagesAsync(chatMessagesDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            if (chatMessagesCreateDto.File != null)
            {
                ChatAttachmentDTO chatAttachmentDTO = await saveFileInLocalAsync(chatMessagesCreateDto.File, result.Data);
                StoredProcedureResult<long> attachmentResult = await _chatAttachmentsRepository.CreateChatAttachmentAsync(chatAttachmentDTO);
                if (!(attachmentResult.ReturnValue > 0))
                {
                    throw attachmentResult.ReturnValue switch
                    {
                        (int)StatusCode.InternalServerError => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
                        _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"])
                    };
                }
                if (!string.IsNullOrWhiteSpace(chatAttachmentDTO.FilePath))
                {
                    string baseUrl = SystemConstant.API_BASE_URL;
                    chatMessagesDto.FilePath = $"{baseUrl}/{chatAttachmentDTO.FilePath
                        .Replace(SystemConstant.ROOT_FOLDER_Path, "")
                        .Replace("\\", "/")}";
                    chatMessagesDto.OriginalFileName = chatAttachmentDTO.OriginalFileName;
                }
            }

            chatMessagesDto.Id = result.Data;
            return chatMessagesDto;
        }

        /// <summary>
        /// Save File In Local Async
        /// </summary>
        /// <param name="file"></param>
        /// <param name="chatMessageId"></param>
        /// <param name="attachmentId"></param>
        /// <returns>
        /// Returns the <see cref="ChatAttachmentDTO"/> representing the saved file attachment. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        private async Task<ChatAttachmentDTO> saveFileInLocalAsync(IFormFile file, long chatMessageId, int? attachmentId = null)
        {
            try
            {
                string fileExtension = Path.GetExtension(file.FileName);
                string guid = Guid.NewGuid().ToString();
                string folderName = guid[..2];
                string folderPath = Path.Combine(SystemConstant.ROOT_FOLDER, SystemConstant.UPLOADS_FOLDER, folderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string uniqueFileName = $"{guid}{fileExtension}";
                string filePath = Path.Combine(folderPath, uniqueFileName);

                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return new ChatAttachmentDTO
                {
                    Id = attachmentId ?? 0,
                    ChatMessageId = chatMessageId,
                    OriginalFileName = file.FileName,
                    FileName = uniqueFileName,
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

        /// <summary>
        /// Update Chat Messages
        /// </summary>
        /// <param name="chatMessagesDto"></param>
        /// <returns>
        /// Returns the updated <see cref="ChatMessagesDto"/>. 
        /// </returns>
        public async Task<ChatMessagesDto> UpdateChatMessagesAsync(ChatMessagesDto chatMessagesDto)
        {
            StoredProcedureResult<long> result = await _chatMessagesRepository.CreateOrUpdateChatMessagesAsync(chatMessagesDto);

            if (!(result.ReturnValue > 0))
            {
                throw result.ReturnValue switch
                {
                    (int)StatusCode.InternalServerError => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
                    (int)StatusCode.ChatMessageDataNotFound => new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_MESSAGES"])),
                    (int)StatusCode.ChatMessageEditWindowExpired => new ValidationException(_localizer["CHAT_MESSAGE_EDIT_WINDOW_EXPIRED"]),
                    _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"])
                };

            }

            chatMessagesDto.Id = result.Data;
            return chatMessagesDto;
        }

        /// <summary>
        /// Delete Chat Messages
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns a boolean indicating whether the chat message was successfully deleted. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        public async Task<bool> DeleteChatMessagesAsync(long id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_MESSAGES_ID"]));
            }

            StoredProcedureResult<bool> result = await _chatMessagesRepository.DeleteChatMessagesAsync(id);

            if (!(result.ReturnValue > 0))
            {
                throw result.ReturnValue switch
                {
                    (int)StatusCode.InternalServerError => new InternalServerErrorException(_localizer["INTERNAL_SERVER"]),
                    (int)StatusCode.ChatMessageDataNotFound => new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_MESSAGES"])),
                    (int)StatusCode.ChatMessageDeleteWindowExpired => new ValidationException(_localizer["CHAT_MESSAGE_DELETE_WINDOW_EXPIRED"]),
                    _ => new InternalServerErrorException(_localizer["INTERNAL_SERVER"])
                };
            }
            return true;
        }

        /// <summary>
        /// Get Chat Messages By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="ChatMessagesDto"/> for the specified chat message ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<ChatMessagesDto> GetChatMessagesByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_CHATS_MESSAGES_ID"]));
            }

            ChatMessagesDto? chatMessage = await _chatMessagesRepository.GetChatMessagesByIdAsync(id);

            if (chatMessage == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_CHATS_MESSAGES"]));
            }

            return chatMessage;
        }

        /// <summary>
        ///  Get All Chat Messages By Filter
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchTerm"></param>
        /// <param name="isAgent"></param>
        /// <returns>
        /// Returns a list of <see cref="ChatMessagesDto"/> for the specified chat session ID and optional search term. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<List<ChatMessagesDto>> GetAllChatMessagesByFilterAsync(long id, string? searchTerm, bool isAgent)
        {
            List<ChatMessagesDto> chatMessages = await _chatMessagesRepository.GetAllChatMessagesByFilterAsync(id, searchTerm, isAgent);

            foreach (ChatMessagesDto chatmessage in chatMessages)
            {
                if (chatmessage.MessageType != 1 && !string.IsNullOrWhiteSpace(chatmessage.FilePath))
                {
                    string baseUrl = SystemConstant.API_BASE_URL;
                    chatmessage.FilePath = $"{baseUrl}/{chatmessage.FilePath.Replace(SystemConstant.ROOT_FOLDER_Path, "").Replace("\\", "/")}" ??
                    throw new InternalServerErrorException(_localizer["ERROR_WHILE_FILE_SAVE"]);
                }
            }
            return chatMessages;
        }
        #endregion
    }
}

