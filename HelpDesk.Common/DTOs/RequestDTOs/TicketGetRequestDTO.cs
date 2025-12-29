namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class GetTicketsByProjectRequestDto
    {
        public long ProjectId { get; set; }
        public int UserId { get; set; }
        public int? Category { get; set; }
        public int? Assignee { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? Department { get; set; }
        public IEnumerable<int>? TagIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }
}
