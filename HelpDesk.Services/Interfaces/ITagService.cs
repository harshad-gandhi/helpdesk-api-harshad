using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagResponseDTO>> GetTagsAsync(TagGetRequestDto request);

    Task<TagCreateResponseDTO> CreateTagAsync(TagCreateRequestDTO request);
}
