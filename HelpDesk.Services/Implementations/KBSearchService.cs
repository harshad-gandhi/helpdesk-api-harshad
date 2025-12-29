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

public class KBSearchService(IKBSearchRepository kbSearchRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IKBSearchService
{
    private readonly IKBSearchRepository _kBSearchRepository = kbSearchRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary> Add KB Search Analytics </summary>
    /// <param name="searchAnalytics"></param>
    /// <returns> KbSearchAnalyticsDto </returns>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the KB search analytics.
    /// </exception>
    public async Task<KbSearchAnalyticsDto> AddKbSearchAnalyticsAsync(KbSearchAnalyticsCreateDto searchAnalytics)
    {
        KbSearchAnalyticsDto analyticsDto = _mapper.Map<KbSearchAnalyticsDto>(searchAnalytics);

        KnowledgeBaseResponseDto result = await _kBSearchRepository.SaveKbSearchAnalyticsAsync(analyticsDto);
        if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        analyticsDto.Id = result.ResultId;
        return analyticsDto;
    }
   
    /// <summary> Get KB Search Analytics </summary>
    /// <param name="projectId"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns> IEnumerable<KbSearchAnalyticsDto> </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no KB search analytics data is found for the given criteria.
    /// </exception>
    public async Task<IEnumerable<KbSearchAnalyticsDto>> GetKbSearchAnalyticsAsync(int? projectId = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        IEnumerable<KbSearchAnalyticsDto> result = await _kBSearchRepository.GetKbSearchAnalyticsAsync(projectId, fromDate, toDate);

        if (result == null || !result.Any())
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_KB_SEARCH_ANALYTICS"]]);
        }

        return result;
    }
}
