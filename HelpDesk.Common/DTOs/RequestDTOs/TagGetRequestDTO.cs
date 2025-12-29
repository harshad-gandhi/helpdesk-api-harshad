using System.ComponentModel.DataAnnotations;
using HelpDesk.Common.Constants;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TagGetRequestDto
    {
        [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
        [Display(Name = "FIELD_PROJECT_ID")]
        public int ProjectId { get; set; }

        public string? Search { get; set; }
    }
}