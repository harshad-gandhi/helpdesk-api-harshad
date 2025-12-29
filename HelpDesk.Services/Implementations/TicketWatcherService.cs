using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class TicketWatcherService(ITicketWatcherRepository ticketWatcherRepository, IStringLocalizer<Messages> localizer) : ITicketWatcherService
{
     private readonly ITicketWatcherRepository _ticketWatcherRepository = ticketWatcherRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

     public async Task AddWatcherToTicketAsync(TicketWatcherCreateRequestDTO request)
    {
        StatusCode code = (StatusCode)await _ticketWatcherRepository.CreateTicketWatcherAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        else if( code == StatusCode.WatcherAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_WATCHER"]]);
        }
    }

    public async Task DeleteWatcherFromTicketAsync(TicketWatcherDeleteRequestDto request)
    {
        StatusCode code = (StatusCode)await _ticketWatcherRepository.DeleteTicketWatcherAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
    }
}
