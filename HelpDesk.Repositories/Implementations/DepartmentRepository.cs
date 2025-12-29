using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

/// <summary>
/// Repository for managing departments.
/// Provides methods to retrieve department data for dropdowns.
/// </summary>
public class DepartmentRepository(IDbConnectionFactory connectionFactory) : IDepartmentRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Retrieves a list of departments, optionally filtered by a search term.
    /// </summary>
    /// <param name="search"></param>
    /// <returns>
    /// An enumerable collection of <see cref="DepartmentDto"/> objects representing the departments.
    /// </returns>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(string? search = null)
    {
        const string spName = "usp_departments_get";

        DynamicParameters parameters = new();
        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add("Search", search);
        }
        IEnumerable<DepartmentDto> result = await _baseRepository.QueryAsync<DepartmentDto>(spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );
        return result;
    }
    /// Retrieves a list of departments to populate dropdowns.
    /// </summary>
    /// <returns>
    /// A list of <see cref="DropdownDTO"/> containing department IDs and names.
    /// </returns>
    public async Task<List<DropdownDTO>> GetDepartmentsAsync()
    {
        const string spName = "usp_departments_dropdown_get";

        DynamicParameters parameters = new();

        IEnumerable<DropdownDTO>? result = await _baseRepository.QueryAsync<DropdownDTO>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result.AsList();
    }

    /// <summary>
    /// Retrieves a department by its ID.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <returns>
    /// The <see cref="DepartmentDto"/> if found; otherwise, null.
    /// </returns>
    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int departmentId)
    {
        const string spName = "usp_department_get_by_id";

        DynamicParameters parameters = new();
        parameters.Add("Id", departmentId);

        DepartmentDto? result = await _baseRepository.QueryFirstOrDefaultAsync<DepartmentDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
    
    /// <summary>
    /// Adds a new department or updates an existing one based on the provided <see cref="DepartmentDto"/>.
    /// </summary>
    /// <param name="departmentDto"></param>
    /// <returns>
    /// A <see cref="DepartmentResponseDto"/> containing:
    /// ResultId: The newly created or updated department ID.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<DepartmentResponseDto> AddUpdateDepartmentAsync(DepartmentDto departmentDto)
    {
        const string spName = "usp_department_save";
        DynamicParameters parameters = new();

        parameters.Add("Id", departmentDto.Id);
        parameters.Add("Name", departmentDto.Name);
        parameters.Add("Description", departmentDto.Description);
        if (departmentDto.Id == null || departmentDto.Id == 0)
        {
            parameters.Add("CreatedBy", departmentDto.CreatedBy);
        }
        else
        {
            parameters.Add("UpdatedBy", departmentDto.UpdatedBy);
        }
        parameters.Add("ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new DepartmentResponseDto
        {
            ResultId = parameters.Get<int>("ResultId"),
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }

    /// <summary>
    /// Deletes a department by its ID.
    /// </summary>
    /// <param name="departmentId"></param>
    /// <param name="updatedBy"></param>
    /// <returns>
    /// A <see cref="DepartmentResponseDto"/> containing:
    /// ResultId: The ID of the deleted department.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<DepartmentResponseDto> DeleteDepartmentAsync(int departmentId, int updatedBy)
    {
        const string spName = "usp_department_delete";
        DynamicParameters parameters = new();

        parameters.Add("Id", departmentId);
        parameters.Add("UpdatedBy", updatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new DepartmentResponseDto
        {
            ResultId = departmentId,
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }

}
