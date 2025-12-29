namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TicketCreateRequestDTO
    {
        public string? TicketForwardingEmail { get; set; }
        public int? ProjectId { get; set; }
        public int? PersonId { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long? ChatSessionId { get; set; }
        public int? AssignedTo { get; set; }
        public int? AssignedDepartmentId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int Source { get; set; }
        public int? CreatedBy { get; set; }
    }
}