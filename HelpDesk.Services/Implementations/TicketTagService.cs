using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class TicketTagService(ITicketTagRepository ticketTagRepository, IStringLocalizer<Messages> localizer) : ITicketTagService
{
    private readonly ITicketTagRepository _ticketTagRepository = ticketTagRepository;

    private readonly IStringLocalizer<Messages> _localizer = localizer;

    public async Task AddTagToTicketAsync(TicketTagCreateRequestDto request)
    {
        StatusCode code = (StatusCode)await _ticketTagRepository.CreateTicketTagMappingAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
    }

    public async Task DeleteTicketTagAsync(TicketTagDeleteRequestDto request)
    {
        StatusCode code = (StatusCode)await _ticketTagRepository.DeleteTicketTagMappingAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
    }
}
