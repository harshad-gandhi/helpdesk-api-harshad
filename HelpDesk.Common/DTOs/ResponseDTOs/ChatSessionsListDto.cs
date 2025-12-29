namespace HelpDesk.Common.DTOs.ResponseDTOs
{
    public class ChatSessionsListDto
    {
        public long Id { get; set; } = 0;
        public int ProjectId { get; set; }
        public int PersonId { get; set; }
        public int? UserId { get; set; }
        public byte CountryId { get; set; }
        public string? ChatIpAddress { get; set; }
        public string? HostName { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Browser { get; set; }
        public int ChatSessionStatus { get; set; }
        public bool IsBotAnswered { get; set; }
        public bool IsSpam { get; set; } = false;
        public bool InTrash { get; set; } = false;
        public bool IsDeleted { get; set; }
        public short? ChatRating { get; set; }
        public string? FeedbackText { get; set; } = string.Empty;
        public DateTimeOffset? FeedBackSubmittedAt { get; set; }
        public int? HelpfullAgent { get; set; }
        public int? ResolutionStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
        public string? ProjectName { get; set; }
        public string? PersonName { get; set; }
        public string? PersonEmail { get; set; }
        public string? PersonPhone { get; set; }
        public string? UserName { get; set; }
        public string? CountryName { get; set; }
        public DateTimeOffset? LastMessageAt { get; set; }
    }
}

