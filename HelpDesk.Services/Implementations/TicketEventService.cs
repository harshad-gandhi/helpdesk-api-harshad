using Azure.Core;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class TicketEventService(ITicketEventRepository ticketEventRepository, IStringLocalizer<Messages> localizer, IFileService fileService, IHubContext<TicketHub> hubContext) : ITicketEventService
{
    private readonly ITicketEventRepository _ticketEventRepository = ticketEventRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    private readonly IFileService _fileService = fileService;

    private readonly IHubContext<TicketHub> _hubContext = hubContext;

    public async Task<TicketEventWithAttachmentsDto> CreateTicketEventAsync(TicketEventCreateRequestDTO request)
    {
        // Event type 1 means message
        if (request.EventType == 1 && request.Files != null && request.Files.Count > 0)
        {
            foreach (var file in request.Files)
            {
                TicketEventAttachmentDto fileDetails = await _fileService.GetFileDetails(file);

                request.Attachments.Add(fileDetails);
            }
        }

        var (statusCode, flatEventData) = await _ticketEventRepository.CreateTicketEventAsync(request);

        if ((StatusCode)statusCode == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        var groupedEvent = flatEventData
        .GroupBy(e => e.EventId)
        .Select(g => new TicketEventWithAttachmentsDto
        {
            EventId = g.Key,
            TicketId = g.First().TicketId,
            EventType = g.First().EventType,
            PerformerType = g.First().PerformerType,
            EventText = g.First().EventText,
            Metadata = g.First().Metadata,
            IsInternal = g.First().IsInternal,
            IsDeleted = g.First().IsDeleted,
            CreatedBy = g.First().CreatedBy,
            CreatedByName = g.First().CreatedByName,
            CreatedAt = g.First().CreatedAt,
            Attachments = g.Where(a => a.AttachmentId != null)
                           .Select(a => new AttachmentDto
                           {
                               AttachmentId = a.AttachmentId,
                               Filename = a.Filename,
                               OriginalFilename = a.OriginalFilename,
                               FilePath = a.FilePath,
                               MimeType = a.MimeType,
                               FileSizeBytes = a.FileSizeBytes
                           }).ToList()
        })
        .FirstOrDefault();

        if(groupedEvent == null)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        if (request.EventType == 1)
        {
            await _hubContext.Clients
                .Group($"Ticket-{request.TicketId}")
                .SendAsync("ReceiveTicketEvent", groupedEvent);
        }

        return groupedEvent!;
    }

}
