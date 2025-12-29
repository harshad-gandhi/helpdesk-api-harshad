namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class ReportingRequestChatDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ProjectId { get; set; }
    }
}