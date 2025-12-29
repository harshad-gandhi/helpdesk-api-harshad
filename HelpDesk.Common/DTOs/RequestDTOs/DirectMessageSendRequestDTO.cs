using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class DirectMessageSendRequestDTO
    {

        public int SenderId { get; set; }

        [Display(Name = "Receiver Id")]
        [Required(ErrorMessage = "REQUIRED")]
        [Range(1, int.MaxValue, ErrorMessage = "ID_POSITIVE")]
        public int ReceiverId { get; set; }

        [Display(Name = "Direct Message")]
        // [Required(ErrorMessage = "REQUIRED")]
        [StringLength(250, ErrorMessage = "LENGTH_LIMIT_250")]
        public string? DirectMessage { get; set; }

        public int MessageType { get; set; } 

        public IFormFile? File { get; set; }
    }
}
