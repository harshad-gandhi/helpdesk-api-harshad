using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class RegisterRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [StringLength(30, ErrorMessage = "MAX_LENGTH_EXCEEDED")]
    [Display(Name = "FIELD_FIRST_NAME")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [StringLength(30, ErrorMessage = "MAX_LENGTH_EXCEEDED")]
    [Display(Name = "FIELD_LAST_NAME")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [EmailAddress(ErrorMessage = "INVALID_EMAIL_FORMAT")]
    [StringLength(70, ErrorMessage = "MAX_LENGTH_EXCEEDED")]
    [Display(Name = "FIELD_EMAIL")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "INVALID_PASSWORD_FORMAT")]
    [Display(Name = "FIELD_PASSWORD")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_TOKEN")]
    public string InviteToken { get; set; } = string.Empty;
}
