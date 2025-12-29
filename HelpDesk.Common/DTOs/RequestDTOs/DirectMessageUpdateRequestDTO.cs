using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class DirectMessageUpdateRequestDTO
    {
        [Display(Name = "Message Id")]
        [Required(ErrorMessage = "REQUIRED")]
        [Range(1, long.MaxValue, ErrorMessage = "ID_POSITIVE")]
        public long MessageId { get; set; }

        [Display(Name = "Direct Message")]
        [Required(ErrorMessage = "REQUIRED")]
        [StringLength(250, ErrorMessage = "LENGTH_LIMIT_250")]
        public string? Message { get; set; }

    }
}