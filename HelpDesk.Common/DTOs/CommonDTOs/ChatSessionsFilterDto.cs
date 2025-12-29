namespace HelpDesk.Common.DTOs.CommonDTOs
{
    public class ChatSessionsFilterDto
    {
        public int? ProjectId { get; set; }
        public int? PersonId { get; set; }
        public int? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int? ChatSessionStatus { get; set; }
        public int? HelpfulAgentStatus { get; set; }
        public int? ResolutionStatus { get; set; }
        public bool IsSpam { get; set; } = false;
        public bool InTrash { get; set; } = false;
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; } = null;
        public string? SortDirection { get; set; } = null;
        public bool IsAssignedToUserActive { get; set; }

    }

}

