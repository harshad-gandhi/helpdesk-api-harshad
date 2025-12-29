using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HelpDesk.Common.DTOs.RequestDTOs;

public class UserProfileRequestDTO
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

    // [RegularExpression(@"^\+[1-9]\d{6,14}$", ErrorMessage = "INVALID_PHONE_NUMBER")]
    // [Display(Name = "FIELD_PHONE_NUMBER")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "FIELD_PROFILE_IMAGE")]
    public IFormFile? ImageFile { get; set; }

    // Optional - path to store in DB
    [StringLength(70, ErrorMessage = "INVALID_DATA")]
    public string? ProfileImagePath { get; set; }
}
