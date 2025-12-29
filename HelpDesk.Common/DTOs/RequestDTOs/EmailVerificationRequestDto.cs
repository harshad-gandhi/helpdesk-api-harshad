using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class EmailVerificationRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_TOKEN")]
    public string Token { get; set; } = string.Empty;
}
