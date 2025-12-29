using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<TagResponseDTO>> GetTagsByProjectAndSearchAsync(TagGetRequestDto request);

    Task<TagCreateResponseDTO?> CreateTagAsync(TagCreateRequestDTO request);
}
