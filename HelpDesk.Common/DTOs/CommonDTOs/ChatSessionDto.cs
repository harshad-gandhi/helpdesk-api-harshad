using System.ComponentModel;

namespace HelpDesk.Common.DTOs.CommonDTOs
{
    public class ChatSessionDto
    {
        public long Id { get; set; } = 0;
        public int ProjectId { get; set; }
        public int PersonId { get; set; }
        public int? UserId { get; set; }
        public byte CountryId { get; set; }
        [DefaultValue(null)]
        public string? ChatIpAddress { get; set; }
        [DefaultValue(null)]
        public string? HostName { get; set; }
        [DefaultValue(null)]
        public string? OperatingSystem { get; set; }
        [DefaultValue(null)]
        public string? Browser { get; set; }
        public int ChatSessionStatus { get; set; }
         [DefaultValue(false)]
        public bool IsBotAnswered { get; set; }
        [DefaultValue(false)]
        public bool IsSpam { get; set; } = false;
        [DefaultValue(false)]
        public bool InTrash { get; set; } = false;
         [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        public short? ChatRating { get; set; }
        [DefaultValue(null)]
        public string? FeedbackText { get; set; } = string.Empty;
        public DateTimeOffset? FeedBackSubmittedAt { get; set; }
        public int? HelpfullAgent { get; set; }
        public int? ResolutionStatus { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? EndedAt { get; set; }
    }

}

