using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(string? search = null);
    Task<DepartmentDto?> GetDepartmentByIdAsync(int departmentId);
    Task<DepartmentResponseDto> AddUpdateDepartmentAsync(DepartmentDto departmentDto);
    Task<DepartmentResponseDto> DeleteDepartmentAsync(int departmentId, int updatedBy);
    Task<List<DropdownDTO>> GetDepartmentsAsync();
}
