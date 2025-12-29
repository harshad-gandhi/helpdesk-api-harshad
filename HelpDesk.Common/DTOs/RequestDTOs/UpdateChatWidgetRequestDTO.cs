using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class UpdateChatWidgetRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Range(1, int.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_PROJECT_ID")]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_WIDGET_SETTING")]
    public string WidgetSetting { get; set; } = string.Empty;

    public int UserId { get; set; }
}
