namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class OrganizationsCreateDto
    {
        public int ProjectId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

}

