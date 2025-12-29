using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class ForgotPasswordRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [EmailAddress(ErrorMessage = "INVALID_EMAIL_FORMAT")]
    [StringLength(70, ErrorMessage = "MAX_LENGTH_EXCEEDED")]
    [Display(Name = "FIELD_EMAIL")]
    public string Email { get; set; } = string.Empty;
}
