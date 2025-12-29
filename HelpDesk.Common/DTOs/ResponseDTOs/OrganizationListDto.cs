namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class OrganizationListDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string ProjectName { get; set; } = string.Empty;

    }

}

