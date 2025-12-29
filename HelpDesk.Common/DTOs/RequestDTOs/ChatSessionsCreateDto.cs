namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class ChatSessionsCreateDto
    {
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
        public DateTimeOffset CreatedAt { get; set; }
    }

}

