using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.SignalR;

namespace HelpDesk.Services.Implementations;

public class TicketService(ITicketRepository ticketRepository, IStringLocalizer<Messages> localizer, IDepartmentRepository departmentRepository, IProjectRepository projectRepository, ITicketEventRepository ticketEventRepository, ITicketTagRepository ticketTagRepository, ITicketWatcherRepository ticketWatcherRepository, IFileService fileService, IHubContext<TicketHub> hubContext) : ITicketService
{

    private readonly ITicketRepository _ticketRepository = ticketRepository;

    private readonly IStringLocalizer<Messages> _localizer = localizer;

    private readonly IDepartmentRepository _departmentRepository = departmentRepository;

    private readonly IProjectRepository _projectRepository = projectRepository;

    private readonly ITicketEventRepository _ticketEventRepository = ticketEventRepository;

    private readonly ITicketTagRepository _ticketTagRepository = ticketTagRepository;

    private readonly ITicketWatcherRepository _ticketWatcherRepository = ticketWatcherRepository;

    private readonly IFileService _fileService = fileService;

    private readonly IHubContext<TicketHub> _hubContext = hubContext;

    #region Tickets
    public async Task<long> CreateTicketAsync(TicketCreateRequestDTO request)
    {
        TicketCreateResponseDTO? response = await _ticketRepository.CreateTicketAsync(request);

        if (response == null || response.StatusCode == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        else if (response.StatusCode == StatusCode.PersonBanned)
        {
            throw new BadRequestException(_localizer["PERSON_BANNED"]);
        }

        return response.TicketId;
    }

    public async Task UpdateTicketAsync(TicketUpdateRequestDTO request)
    {
        StatusCode code = (StatusCode)await _ticketRepository.UpdateTicketAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        else if (code == StatusCode.InvalidColumnCode)
        {
            throw new BadRequestException(_localizer["INVALID_COLUMN_CODE"]);
        }
    }

    public async Task UpdateMultipleTicketsAsync(MultipleTicketUpdateDTO request)
    {
        StatusCode code = (StatusCode)await _ticketRepository.UpdateMultipleTicketsAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
    }


    public async Task DeleteTicketAsync(TicketDeleteRequestDTO request)
    {
        StatusCode code = (StatusCode)await _ticketRepository.DeleteTicketAsync(request);

        if (code == StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
    }

    public async Task<TicketDetailsDto> GetTicketByIdAsync(long ticketId)
    {
        var flatData = await _ticketRepository.GetTicketByIdAsync(ticketId);

        var groupedEvents = flatData.EventsWithAttachments
            .GroupBy(e => e.EventId)
            .Select(group => new TicketEventWithAttachmentsDto
            {
                EventId = group.Key,
                TicketId = group.First().TicketId,
                EventType = group.First().EventType,
                PerformerType = group.First().PerformerType,
                EventText = group.First().EventText,
                Metadata = group.First().Metadata,
                IsInternal = group.First().IsInternal,
                IsDeleted = group.First().IsDeleted,
                CreatedBy = group.First().CreatedBy,
                CreatedByName = group.First().CreatedByName,
                CreatedAt = group.First().CreatedAt,
                Attachments = group
                    .Where(a => a.AttachmentId != null)
                    .Select(a => new AttachmentDto
                    {
                        AttachmentId = a.AttachmentId,
                        Filename = a.Filename,
                        OriginalFilename = a.OriginalFilename,
                        FilePath = a.FilePath,
                        MimeType = a.MimeType,
                        FileSizeBytes = a.FileSizeBytes
                    })
                    .ToList()
            })
            .ToList();

        return new TicketDetailsDto
        {
            TicketInfo = flatData.TicketInfo,
            Tags = flatData.Tags,
            EventsWithoutAttachments = flatData.EventsWithoutAttachments,
            Watchers = flatData.Watchers,
            EventsWithAttachments = groupedEvents
        };
    }

    public async Task<PagedTicketWithTagsDto> GetPaginatedTicketsAsync(GetTicketsByProjectRequestDto request)
    {
        var (tickets, tags, totalCount) = await _ticketRepository.GetPaginatedTicketsAsync(request);

        var result = tickets.Select(t => new TicketWithTagsDto
        {
            Ticket = t,
            Tags = tags
                .Where(tag => tag.TicketId == t.TicketId)
                .Select(tag => new TagResponseDTO { TagId = tag.TagId, TagName = tag.TagName })
                .ToList()
        }).ToList();

        return new PagedTicketWithTagsDto
        {
            Tickets = result,
            TotalCount = totalCount
        };
    }

    public async Task<List<DropdownDTO>> GetDepartmentsAsync()
    {
        return await _departmentRepository.GetDepartmentsAsync();
    }

    public async Task<List<DropdownDTO>> GetProjectsByUserIdAsync(int userId)
    {
        return await _projectRepository.GetProjectsByUserIdAsync(userId);
    }

    public async Task<List<DepartmentUserListResponseDTO>> GetProjectMembersByProjectIdAsync(int projectId)
    {
        List<UserListResponseDTO>? users = await _ticketRepository.GetUserListAsync(projectId);

        List<DepartmentUserListResponseDTO>? groupedUsers = users
            .GroupBy(u => string.IsNullOrWhiteSpace(u.DepartmentName) ? "Admin" : u.DepartmentName)
            .Select(g => new DepartmentUserListResponseDTO
            {
                DepartmentName = g.Key,
                Users = g.ToList()
            })
            .ToList();

        return groupedUsers;
    }

    #endregion
    #region Ticket Events
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

        if (groupedEvent == null)
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
    #endregion
    #region Ticket Tags

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

    #endregion
    #region Ticket Watchers

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
    
    #endregion
}
