using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class InviteProjectMemberRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [EmailAddress(ErrorMessage = "INVALID_DATA")]
    [StringLength(70, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_EMAIL")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Range(1, byte.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_ROLE")]
    public byte RoleId { get; set; } 

    [Range(1, int.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_DEPARTMENT")]
    public int? DepartmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_REPORTS_TO")]
    public int ReportsToId { get; set; }   

    public int CreatedBy { get; set; }
}
