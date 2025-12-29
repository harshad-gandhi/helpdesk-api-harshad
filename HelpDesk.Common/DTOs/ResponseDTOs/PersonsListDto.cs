using System.ComponentModel;

namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class PersonsListDto
    {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int? OrganizationId { get; set; }
        public int? CountryId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        [DefaultValue(false)]
        public bool IsBlocked { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public DateTimeOffset? FirstChatAt { get; set; }
        public DateTimeOffset? LastSeenAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset CateatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
    }

}

