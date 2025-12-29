using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class UpdateProjectRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Range(1, int.MaxValue, ErrorMessage = "PARAMETER_GREATER_THAN_ZERO")]
    [Display(Name = "FIELD_PROJECT_ID")]
    public int Id { get; set; }

    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [StringLength(70, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_PROJECT_NAME")]
    public string Name { get; set; } = null!;

    public int UpdatedBy { get; set; }

    [Url(ErrorMessage = "INVALID_DATA")]
    [StringLength(512, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_LIVE_PROJECT_URL")]
    public string? LiveProjectUrl { get; set; }

    [StringLength(255, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_DESCRIPTION")]
    public string? Description { get; set; }

    // Remove base64 string
    [Display(Name = "FIELD_PROJECT_IMAGE")]
    public IFormFile? ImageFile { get; set; }

    // Optional - path to store in DB
    [StringLength(70, ErrorMessage = "INVALID_DATA")]
    public string? ProjectImagePath { get; set; }

    [Display(Name = "FIELD_SETTINGS")]
    public string? Settings { get; set; }
    
    [Display(Name = "FIELD_IS_PROJECT_ENABLE")]
    public bool? IsProjectEnable { get; set; }

    [Display(Name = "FIELD_IS_PRECHAT_FORM_ENABLE")]
    public bool? IsPrechatFormEnable { get; set; }
}
