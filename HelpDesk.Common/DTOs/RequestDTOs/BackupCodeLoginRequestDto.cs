using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class BackupCodeLoginRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_USER_ID")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_BACKUP_CODE")]
    public string BackupCode { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; }
}
