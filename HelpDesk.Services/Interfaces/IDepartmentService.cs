using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(string? search = null);
    Task<DepartmentDto?> GetDepartmentByIdAsync(int departmentId);
    Task<DepartmentDto> AddDepartmentAsync(DepartmentCreateDto departmentCreateDto);
     Task<DepartmentDto> UpdateDepartmentAsync(DepartmentUpdateDto departmentUpdateDto);
     Task<DepartmentResponseDto> DeleteDepartmentAsync(int departmentId, int updatedBy);
}
