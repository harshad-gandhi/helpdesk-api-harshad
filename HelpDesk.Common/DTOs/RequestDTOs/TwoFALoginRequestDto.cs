using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class TwoFALoginRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_USER_ID")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_2FA_CODE")]
    public string Code { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; }
}
