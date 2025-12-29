using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class DirectMessageMarkAllAsReadRequestDTO
    {

        [Display(Name = "Sender Id")]
        [Required(ErrorMessage = "REQUIRED")]
        [Range(1, int.MaxValue, ErrorMessage = "ID_POSITIVE")]
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

    }
}