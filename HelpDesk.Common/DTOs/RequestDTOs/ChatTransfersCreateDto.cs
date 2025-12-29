namespace HelpDesk.Common.DTOs.RequestDTOs
{

    public class ChatTransfersCreateDto
    {
     public long ChatSessionId { get; set; }
        public int? FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int TransferType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTimeOffset TranferredAt { get; set; }
}

}
