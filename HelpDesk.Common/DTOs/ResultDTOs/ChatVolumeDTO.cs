namespace HelpDesk.Common.DTOs.ResultDTOs
{
    public class ChatVolumeDTO
    {
        public DateTime Chat_Date { get; set; }

        public int Total_Chats { get; set; }

        public int Resolved_Chats { get; set; }
    }
}