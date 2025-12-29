namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    // Ticket Info (Single Row)
    public class TicketInfoDto
    {
        public long TicketId { get; set; }
        public int ProjectId { get; set; }
        public long PersonId { get; set; }
        public string? PersonName { get; set; }
        public long OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public long? ChatSessionId { get; set; }
        public int? AssignedTo { get; set; } // ProjectMemberId
        public string? AgentName { get; set; }
        public int? AssignedDepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? MergedInto { get; set; }
        public string? TicketNumber { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int Source { get; set; }
        public int? SatisfactionRating { get; set; }
        public string? Comment { get; set; }
        public bool IsMerged { get; set; }
        public bool IsSpam { get; set; }
        public bool InTrash { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? AssignedAt { get; set; }
        public DateTimeOffset? FirstResponseAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public int CreatedBy { get; set; } // ProjectMemberId
        public string? CreatedByName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class AttachmentDto
    {
        public long? AttachmentId { get; set; }
        public string? Filename { get; set; }
        public string? OriginalFilename { get; set; }
        public string? FilePath { get; set; }
        public int? MimeType { get; set; }
        public long? FileSizeBytes { get; set; }
    }

    public class TicketEventWithAttachmentsDto
    {
        public long EventId { get; set; }
        public long TicketId { get; set; }
        public int EventType { get; set; }
        public int PerformerType { get; set; }
        public string? EventText { get; set; }
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public List<AttachmentDto>? Attachments { get; set; }
    }

    //For repo

    public class TicketEventWithAttachmentDto
    {
        public long EventId { get; set; }
        public long TicketId { get; set; }
        public int EventType { get; set; }
        public int PerformerType { get; set; }
        public string? EventText { get; set; }
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public long? AttachmentId { get; set; }
        public string? Filename { get; set; }
        public string? OriginalFilename { get; set; }
        public string? FilePath { get; set; }
        public int? MimeType { get; set; }
        public long? FileSizeBytes { get; set; }
    }

    // Ticket Events without Attachments
    public class TicketEventDto
    {
        public long EventId { get; set; }
        public long TicketId { get; set; }
        public byte EventType { get; set; }
        public byte PerformerType { get; set; }
        public string? EventText { get; set; }
        public string? Metadata { get; set; }
        public bool IsInternal { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    // Ticket Watchers
    public class TicketWatcherDto
    {
        public long WatcherId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    // Master DTO
    public class TicketDetailsDto
    {
        public TicketInfoDto TicketInfo { get; set; }
        public IEnumerable<TagResponseDTO> Tags { get; set; }
        public IEnumerable<TicketEventWithAttachmentsDto> EventsWithAttachments { get; set; }
        public IEnumerable<TicketEventDto> EventsWithoutAttachments { get; set; }
        public IEnumerable<TicketWatcherDto> Watchers { get; set; }
    }

    public class TicketDetailsFlatDto
{
    public TicketInfoDto TicketInfo { get; set; }
    public IEnumerable<TagResponseDTO> Tags { get; set; }
    public IEnumerable<TicketEventWithAttachmentDto> EventsWithAttachments { get; set; } 
    public IEnumerable<TicketEventDto> EventsWithoutAttachments { get; set; }
    public IEnumerable<TicketWatcherDto> Watchers { get; set; }
}

}
