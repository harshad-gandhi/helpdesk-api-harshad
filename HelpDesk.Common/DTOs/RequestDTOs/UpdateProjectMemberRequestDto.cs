using System.ComponentModel.DataAnnotations;
using HelpDesk.Common.DTOs.CommonDTOs;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class UpdateProjectMemberRequestDTO
{
    public int? AgentUserId { get; set; }
    public int? AdminUserId { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Range(1, byte.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_CHAT_LIMIT")]
    public byte ChatLimit { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_ROLE")]
    public byte Role { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_STATUS")]
    public bool Status { get; set; }

    [Display(Name = "FIELD_DEPARTMENT")]
    public int? Department { get; set; }

    [Display(Name = "FIELD_REPORTS_TO")]
    public int? ReportsToPerson { get; set; }

    public List<int> AdminProjects { get; set; } = [];

    public List<AgentProjectAssignDTO> AgentProjects { get; set; } = [];

    public int UpdatedBy { get; set; }
}