using Azure.Core;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static HelpDesk.Common.Enums.StatusCode;

namespace HelpDesk.Services.Implementations;

public class TagService(ITagRepository tagRepository, IStringLocalizer<Messages> localizer) : ITagService
{
    private readonly ITagRepository _tagRepository = tagRepository;

    private readonly IStringLocalizer<Messages> _localizer = localizer;

    public async Task<IEnumerable<TagResponseDTO>> GetTagsAsync(TagGetRequestDto request)
    {
        IEnumerable<TagResponseDTO> Taglist = await _tagRepository.GetTagsByProjectAndSearchAsync(request);

        return Taglist;
    }

    public async Task<TagCreateResponseDTO> CreateTagAsync(TagCreateRequestDTO request)
    {
        TagCreateResponseDTO? response = await _tagRepository.CreateTagAsync(request);

        if (response == null || response.StatusCode == InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return response;
    }
}
