using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class DirectMessagesGetRequestDTO
    {

        public int SenderId { get; set; }

        [Display(Name = "Receiver Id")]
        [Required(ErrorMessage = "REQUIRED")]
        [Range(1, int.MaxValue, ErrorMessage = "ID_POSITIVE")]
        public int ReceiverId { get; set; }

        public int PageSize { get; set; } = 5;

        public string KeyWord { get; set; } = string.Empty;

        public int MessageId { get; set; }

        public int? LastMessageId { get; set; }

        public bool Is_Direction_Ascending { get; set; } = false;
    }
}