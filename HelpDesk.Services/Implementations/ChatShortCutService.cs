using AutoMapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations
{
    public class ChatShortCutService(IChatShortCutRepository chatShortCutRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IChatShortCutService
    {
        private readonly IChatShortCutRepository _chatShortCutRepository = chatShortCutRepository;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Retrieves the list of chat shortcuts for a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project for which to fetch chat shortcuts. Must be a positive integer.</param>
        /// <returns>
        /// A <see cref="List{ChatShortCutResponseDTO}"/> containing the mapped chat shortcut details for the project.
        /// </returns>
        /// <exception cref="BadRequestException">Thrown when <paramref name="projectId"/> is less than or equal to zero.</exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure retrieving the chat shortcuts from the repository.
        /// </exception>
        public async Task<List<ChatShortCutResponseDTO>> GetChatShortCutAsync(int projectId)
        {
            if (projectId <= 0)
                throw new BadRequestException(_localizer["ID_POSITIVE", "ID"]);

            List<ChatShortCutResultDTO>? chatShortCutResultDTOs = await _chatShortCutRepository.GetChatShortCutAsync(projectId);

            List<ChatShortCutResponseDTO> chatShortCutResponseDTO = _mapper.Map<List<ChatShortCutResponseDTO>>(chatShortCutResultDTOs);

            return chatShortCutResponseDTO;
        }

        /// <summary>
        /// Creates a new chat shortcut based on the provided details.
        /// </summary>
        /// <param name="chatShortCutCreateRequestDTO">The request DTO containing the details of the chat shortcut to create.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResponseDTO"/> containing the details of the created chat shortcut.
        /// </returns>
        /// <exception cref="DataAlreadyExistsException">
        /// Thrown when a chat shortcut with the same key already exists.
        /// </exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure saving the chat shortcut to the repository.
        /// </exception>
        public async Task<ChatShortCutResponseDTO> CreateChatShortCutAsync(ChatShortCutCreateRequestDTO chatShortCutCreateRequestDTO)
        {
            ChatShortCutResultDTO chatShortCutResultDTO = await _chatShortCutRepository.CreateChatShortCutAsync(chatShortCutCreateRequestDTO);

            if (chatShortCutResultDTO.ResultCode == 409)
            {
                throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_SHORT_CUT_KEY"]]);
            }
            else if (chatShortCutResultDTO.ResultCode == 500)
            {
                throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = _mapper.Map<ChatShortCutResponseDTO>(chatShortCutResultDTO);

            return chatShortCutResponseDTO;
        }

        /// <summary>
        /// Updates an existing chat shortcut with the provided details.
        /// </summary>
        /// <param name="chatShortCutUpdateRequestDTO">The request DTO containing the updated details of the chat shortcut.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResponseDTO"/> containing the details of the updated chat shortcut.
        /// </returns>
        /// <exception cref="DataAlreadyExistsException">
        /// Thrown when a chat shortcut with the same key already exists.
        /// </exception>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure updating the chat shortcut in the repository.
        /// </exception>
        public async Task<ChatShortCutResponseDTO> UpdateChatShortCutAsync(ChatShortCutUpdateRequestDTO chatShortCutUpdateRequestDTO)
        {
            ChatShortCutResultDTO chatShortCutResultDTO = await _chatShortCutRepository.UpdateChatShortCutAsync(chatShortCutUpdateRequestDTO);

            if (chatShortCutResultDTO.ResultCode == 403)
            {
                throw new DataAlreadyExistsException(_localizer["You can not update this."]);
            }
            else if (chatShortCutResultDTO.ResultCode == 409)
            {
                throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_SHORT_CUT_KEY"]]);
            }
            else if (chatShortCutResultDTO.ResultCode == 500)
            {
                throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = _mapper.Map<ChatShortCutResponseDTO>(chatShortCutResultDTO);

            return chatShortCutResponseDTO;
        }

        /// <summary>
        /// Deletes a chat shortcut for the specified user.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResponseDTO"/> containing the details of the deleted chat shortcut.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure deleting the chat shortcut from the repository.
        /// </exception>
        public async Task<ChatShortCutResponseDTO> DeleteChatShortCutAsync(int id, int userId)
        {
            ChatShortCutResultDTO chatShortCutResultDTO = await _chatShortCutRepository.DeleteChatShortCutAsync(id, userId);

            if (chatShortCutResultDTO.ResultCode == 403)
            {
                throw new DataAlreadyExistsException(_localizer["You can not delete this."]);
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = _mapper.Map<ChatShortCutResponseDTO>(chatShortCutResultDTO);

            return chatShortCutResponseDTO;

        }

        /// <summary>
        /// Toggles the visibility (public/private) of a chat shortcut for the specified user.
        /// </summary>
        /// <param name="id">The ID of the chat shortcut whose visibility is to be toggled.</param>
        /// <param name="userId">The ID of the user performing the visibility toggle.</param>
        /// <returns>
        /// A <see cref="ChatShortCutResponseDTO"/> containing the updated details of the chat shortcut.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when there is an internal failure updating the visibility of the chat shortcut in the repository.
        /// </exception>
        public async Task<ChatShortCutResponseDTO> ToggleChatShortCutVisibilityAsync(int id, int userId)
        {
            ChatShortCutResultDTO chatShortCutResultDTO = await _chatShortCutRepository.ToggleChatShortCutVisibilityAsync(id, userId);

            if (chatShortCutResultDTO.ResultCode == 403)
            {
                throw new DataAlreadyExistsException(_localizer["You can not update this."]);
            }

            ChatShortCutResponseDTO chatShortCutResponseDTO = _mapper.Map<ChatShortCutResponseDTO>(chatShortCutResultDTO);

            return chatShortCutResponseDTO;

        }

    }
}