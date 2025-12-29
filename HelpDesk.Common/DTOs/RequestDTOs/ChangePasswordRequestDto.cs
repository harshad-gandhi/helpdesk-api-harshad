using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ChangePasswordRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_CURRENT_PASSWORD")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "INVALID_PASSWORD_FORMAT")]
    [Display(Name = "FIELD_NEW_PASSWORD")]
    public string NewPassword { get; set; } = string.Empty;

    [Compare("NewPassword", ErrorMessage = "PASSWORDS_DO_NOT_MATCH")]
    [Display(Name = "FIELD_CONFIRM_PASSWORD")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
