using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class ChatShortCutCreateRequestDTO
    {
        [Display(Name = "Project Id")]
        [Required(ErrorMessage = "REQUIRED")]
        [Range(1, int.MaxValue, ErrorMessage = "ID_POSITIVE")]
        public int ProjectId { get; set; }

        public int UserId { get; set; }

        [Display(Name = "Short Cut Key")]
        [Required(ErrorMessage = "REQUIRED")]
        [StringLength(250, ErrorMessage = "LENGTH_LIMIT_250")]
        public required string ShortCutKey { get; set; }

        [Display(Name = "Short Cut MEssage")]
        [Required(ErrorMessage = "REQUIRED")]
        [StringLength(250, ErrorMessage = "LENGTH_LIMIT_250")]
        public required string ShortCutMessage { get; set; }

        public bool IsPublic { get; set; } = true;

    }
}