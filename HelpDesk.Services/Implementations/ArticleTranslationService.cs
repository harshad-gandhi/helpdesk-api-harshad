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

public class ArticleTranslationService(IArticleTranslationRepository articleTranslationRepository,IStringLocalizer<Messages> localizer, IMapper mapper) : IArticleTranslationService
{
    private readonly IArticleTranslationRepository _articleTranslationRepository = articleTranslationRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary>Add Article Translation </summary>
    /// <param name="translation"></param>
    /// <returns> ArticleTranslationDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when an article translation with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the article translation.
    /// </exception>
    public async Task<ArticleTranslationDto> AddArticleTranslationAsync(ArticleTranslationCreateDto translation)
    {
        ArticleTranslationDto translationDto = _mapper.Map<ArticleTranslationDto>(translation);
        KnowledgeBaseResponseDto result = await _articleTranslationRepository.AddUpdateArticleTranslationAsync(translationDto);

        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_ARTICLE_TRANSLATION_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        translationDto.Id = result.ResultId;
        return translationDto;
    }
   
    /// <summary> Delete Article Translation </summary>
    /// <param name="translationId"></param>
    /// <returns> KnowledgeBaseResponseDto </returns>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while deleting the article translation.
    /// </exception>
    public async Task<KnowledgeBaseResponseDto> DeleteArticleTranslationAsync(int translationId)
    {

        KnowledgeBaseResponseDto result = await _articleTranslationRepository.DeleteArticleTranslationAsync(translationId);
        if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return result;
    }

}

