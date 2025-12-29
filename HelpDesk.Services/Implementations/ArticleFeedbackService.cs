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

public class ArticleFeedbackService(IArticleFeedbackRepository articleFeedbackRepository,IStringLocalizer<Messages> localizer,IMapper mapper) : IArticleFeddbackService
{
    private readonly IArticleFeedbackRepository _articleFeedbackRepository = articleFeedbackRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Add Article Feedback
    /// </summary>
    /// <param name="articleFeedback"></param>
    /// <returns>
    /// ArticleFeedbackDto
    /// </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when an article feedback with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the article feedback.
    /// </exception>
    public async Task<ArticleFeedbackDto> AddArticleFeedback(ArticleFeedbackCreateDto articleFeedback)
    {
        ArticleFeedbackDto feedbackDto = _mapper.Map<ArticleFeedbackDto>(articleFeedback);

        KnowledgeBaseResponseDto result = await _articleFeedbackRepository.AddArticleFeedback(feedbackDto);
        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_ARTICLE_FEEDBACK_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        feedbackDto.Id = result.ResultId;
        return feedbackDto;
    }


}
