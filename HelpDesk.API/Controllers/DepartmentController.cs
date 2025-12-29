using System.Net;
using System.Security.Claims;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DepartmentController(IDepartmentService departmentService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
{
    private readonly IDepartmentService _departmentService = departmentService;
    private readonly IResponseService<object> _responseService = responseService;
    private readonly IStringLocalizer<Messages> _localizer = localizer;

    /// <summary>
    /// Get all departments with optional search parameter
    /// </summary>
    /// <param name="search"></param>
    [HttpGet("get-departments")]
    public async Task<IActionResult> GetDepartments([FromQuery] string? search = null)
    {
        IEnumerable<DepartmentDto> result = await _departmentService.GetDepartmentsAsync(search);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_DEPARTMENT"]]]);
    }
    
    /// <summary>
    /// Get department by ID
    /// </summary>
    /// <param name="departmentId"></param>
    [HttpGet("get-department-by-id/{departmentId}")]

    public async Task<IActionResult> GetDepartmentById([FromRoute] int departmentId)
    {
        DepartmentDto? result = await _departmentService.GetDepartmentByIdAsync(departmentId);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_FETCH_SUCCEED", _localizer["FIELD_DEPARTMENT"]]]);
    }
    
    /// <summary>
    /// Add a new department
    /// </summary>
    /// <param name="departmentCreateDto"></param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user ID is not found in the claims.
    /// </exception>
    [HttpPost("add-department")]
    public async Task<IActionResult> AddDepartment([FromBody] DepartmentCreateDto departmentCreateDto)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        departmentCreateDto.CreatedBy = int.Parse(UserId);
        DepartmentDto result = await _departmentService.AddDepartmentAsync(departmentCreateDto);
        return _responseService.GetSuccessResponse(HttpStatusCode.Created, result, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_DEPARTMENT"]]]);
    }

    /// <summary>
    /// Update an existing department
    /// </summary>
    /// <param name="departmentUpdateDto"></param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user ID is not found in the claims.
    /// </exception>
    [HttpPut("update-department")]
    public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentUpdateDto departmentUpdateDto)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        departmentUpdateDto.UpdatedBy = int.Parse(UserId);
        DepartmentDto result = await _departmentService.UpdateDepartmentAsync(departmentUpdateDto);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_UPDATION_SUCCEED", _localizer["FIELD_DEPARTMENT"]]]);
    }

    /// <summary>
    /// Delete a department by ID
    /// </summary>
    /// <param name="departmentId"></param>
    /// <exception cref="UnauthorizedAccessException">
    /// Thrown when the user ID is not found in the claims.
    /// </exception>
    [HttpDelete("delete-department/{departmentId}")]

    public async Task<IActionResult> DeleteDepartment([FromRoute] int departmentId)
    {
        string? UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (UserId == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
        int updatedBy = int.Parse(UserId);
        DepartmentResponseDto result = await _departmentService.DeleteDepartmentAsync(departmentId, updatedBy);
        return _responseService.GetSuccessResponse(HttpStatusCode.OK, result, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_DEPARTMENT"]]]);

    }
}
