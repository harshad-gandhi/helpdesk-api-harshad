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

public class ArticleService(IArticleRepository articleRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IArticleService
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary> Add Article </summary>
    /// <param name="article"></param>
    /// <returns> ArticleCreateDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when an article with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the article.
    /// </exception>
    public async Task<ArticleCreateDto> AddArticleAsync(ArticleRequestDto article)
    {
        ArticleCreateDto articleDto = _mapper.Map<ArticleCreateDto>(article);

        KnowledgeBaseResponseDto result = await _articleRepository.AddUpdateArticleAsync(articleDto);

        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_ARTICLE_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        articleDto.Id = result.ResultId;
        return articleDto;
    }

    /// <summary> Update Article </summary>
    /// <param name="article"></param>
    /// <returns> ArticleCreateDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when an article with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while updating the article.
    /// </exception>
    public async Task<ArticleCreateDto> UpdateArticleAsync(ArticleUpdateDto article)
    {
        ArticleCreateDto articleDto = _mapper.Map<ArticleCreateDto>(article);
        KnowledgeBaseResponseDto result = await _articleRepository.AddUpdateArticleAsync(articleDto);

        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_ARTICLE_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        articleDto.Id = result.ResultId;
        return articleDto;
    }
    
    /// <summary> Get Article By Id </summary>
    /// <param name="articleId"></param>
    /// <returns> ArticleDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the articleId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the article with the specified ID is not found.
    /// </exception>
    public async Task<ArticleDto> GetArticleByIdAsync(int articleId)
    {
        if (articleId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_ARTILCE"]]);
        }

        ArticleDto? result = await _articleRepository.GetArticleByIdAsync(articleId);

        if (result == null)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ARTICLE"]]);
        }

        return result;
    }

    /// <summary> Get Articles </summary>
    /// <param name="filter"></param>
    /// <returns> IEnumerable<ArticleDto> </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no articles are found matching the specified filter criteria.
    /// </exception>
    public async Task<IEnumerable<ArticleDto>> GetArticlesAsync(ArticleFilterDto filter)
    {
        IEnumerable<ArticleDto> result = await _articleRepository.GetArticlesAsync(filter);

        if (result == null || !result.Any())
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ARTICLE"]]);
        }

        return result;
    }

    /// <summary> Delete Article </summary>
    /// <param name="articleId"></param>
    /// <param name="updatedBy"></param>
    /// <returns> KnowledgeBaseResponseDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the articleId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the article with the specified ID is not found.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while deleting the article.
    /// </exception>
    public async Task<KnowledgeBaseResponseDto> DeleteArticleAsync(int articleId, int updatedBy)
    {
        if (articleId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_ARTICLE"]]);
        }

        KnowledgeBaseResponseDto result = await _articleRepository.DeleteArticleAsync(articleId, updatedBy);

        if (result.ReturnValue == (int)StatusCode.NotFound)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ARTICLE"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return result;
    }

    /// <summary> Archive Article </summary>
    /// <param name="articleId"></param>
    /// <param name="updatedBy"></param>
    /// <returns> KnowledgeBaseResponseDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the articleId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the article with the specified ID is not found.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while archiving the article.
    /// </exception>
    public async Task<KnowledgeBaseResponseDto> ArchiveArticleAsync(int articleId, int updatedBy)
    {
        if (articleId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_ARTILCE"]]);
        }

        KnowledgeBaseResponseDto result = await _articleRepository.ArchiveArticleAsync(articleId, updatedBy);

        if (result.ReturnValue == (int)StatusCode.NotFound)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_ARTICLE"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return result;
    }
}
