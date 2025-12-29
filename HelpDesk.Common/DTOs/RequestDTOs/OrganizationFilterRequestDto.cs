    namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class OrganizationFilterRequestDto
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public string? SortDirection { get; set; } = null;
        public int? ProjectId { get; set; }
    }

}

