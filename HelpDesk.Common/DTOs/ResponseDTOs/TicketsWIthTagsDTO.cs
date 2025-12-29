namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class TicketDto
    {
        public long TicketId { get; set; }
        public long ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public long PersonId { get; set; }
        public string? PersonName { get; set; }
        public long? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        public string? Email { get; set; }

        public string? TicketNumber { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? Source { get; set; }

        public long? AssigneeId { get; set; }

        public string? AssigneeName { get; set; }
        public bool IsSpam { get; set; }
        public bool InTrash { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class TicketTagDto
    {
        public long TicketId { get; set; }
        public int TagId { get; set; }
        public string? TagName { get; set; }
    }

    public class TicketWithTagsDto
    {
        public TicketDto Ticket { get; set; } = new TicketDto();
        public List<TagResponseDTO> Tags { get; set; } = [];
    }

    public class PagedTicketWithTagsDto
    {
        public List<TicketWithTagsDto> Tickets { get; set; }
        public int TotalCount { get; set; }
    }

}
