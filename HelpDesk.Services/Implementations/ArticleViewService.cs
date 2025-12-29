using AutoMapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class ArticleViewService(IArticleViewRepository articleViewRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IArticleViewService
{
    private readonly IArticleViewRepository _articleViewRepository = articleViewRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary> Add Article View </summary>
    /// <param name="articleView"></param>
    /// <returns> ArticleViewDto </returns>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the article view.
    /// </exception>
    public async Task<ArticleViewDto> AddArticleView(ArticleViewCreateDto articleView)
    {
        ArticleViewDto viewDto = _mapper.Map<ArticleViewDto>(articleView);

        KnowledgeBaseResponseDto result = await _articleViewRepository.AddArticleView(viewDto);
        if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        viewDto.Id = result.ResultId;
        return viewDto;
    }

}
