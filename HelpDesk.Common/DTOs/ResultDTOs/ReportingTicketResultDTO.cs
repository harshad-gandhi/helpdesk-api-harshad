namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class ReportingTicketResultDTO
    {
        public int TotalTickets { get; set; }

        public int ResolvedTickets { get; set; }

        public double AvgResolutionTimeSeconds { get; set; }

        public double OpenTickets { get; set; }

        public double PrevTotalTickets { get; set; }

        public int CurTotalTickets { get; set; }

        public string? ProjectName { get; set; }
    }
}