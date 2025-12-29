using System.ComponentModel.DataAnnotations;
using HelpDesk.Common.Constants;

namespace HelpDesk.Common.DTOs.RequestDTOs
{
    public class TagCreateRequestDTO
    {
        [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
        [Range(1, int.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
        [Display(Name = "FIELD_PROJECT_ID")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
        [Display(Name = "FIELD_TAG_NAME")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
        [Display(Name = "FIELD_CREATED_BY")]
        public int CreatedBy { get; set; }
    }
}