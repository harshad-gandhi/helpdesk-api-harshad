using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class AddProjectRequestDTO
{
    [Required(ErrorMessage = "PARAMETER_NOT_NULL")]
    [Display(Name = "FIELD_PROJECT_NAME")]
    public string Name { get; set; } = string.Empty;

    public int CreatedBy { get; set; }

    [Url(ErrorMessage = "INVALID_DATA")]
    [StringLength(512, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_LIVE_PROJECT_URL")]
    public string? LiveProjectUrl { get; set; }

    [StringLength(255, ErrorMessage = "INVALID_DATA")]
    [Display(Name = "FIELD_DESCRIPTION")]
    public string? Description { get; set; }

    [Display(Name = "FIELD_PROJECT_IMAGE")]
    public IFormFile? ImageFile { get; set; }

    // Optional - path to store in DB
    [StringLength(70, ErrorMessage = "INVALID_DATA")]
    public string? ProjectImagePath { get; set; }
}
