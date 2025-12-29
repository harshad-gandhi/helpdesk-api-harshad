using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [EmailAddress(ErrorMessage = "INVALID_EMAIL_FORMAT")]
    [StringLength(70, ErrorMessage = "MAX_LENGTH_EXCEEDED")]
    [Display(Name = "FIELD_EMAIL")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_PASSWORD")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;

    // [Required(ErrorMessage = "CAPTCHA_REQUIRED")]
    // public string TurnstileToken { get; set; } = string.Empty; 
}
