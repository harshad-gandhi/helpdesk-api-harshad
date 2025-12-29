namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class TicketPriorityDistributionDTO
    {

        public int Priority { get; set; }

        public int TotalTickets { get; set; }

        public int OpenTickets { get; set; }

        public int OverallTotal { get; set; }

        public string? PriorityName { get; set; }

    }
}