namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class TicketTrendsDTO
    {
        public DateTime Date { get; set; }

        public int Created { get; set; }

        public int Resolved { get; set; }

        public int Closed { get; set; }
    }
}