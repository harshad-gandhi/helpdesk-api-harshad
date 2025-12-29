namespace HelpDesk.Common.DTOs.RequestDTOs
{
public class PersonsCreateDto
{
        public int ProjectId { get; set; }
        public int? OrganizationId { get; set; }
        public int? CountryId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public DateTimeOffset? FirstChatAt { get; set; }
        public DateTimeOffset? LastSeenAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CateatedAt { get; set; }
}

}

