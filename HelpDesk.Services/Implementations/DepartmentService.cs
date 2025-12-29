using AutoMapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class DepartmentService(IDepartmentRepository departmentRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository = departmentRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary> Get Departments </summary>
    /// <param name="search"></param>
    /// <returns> IEnumerable<DepartmentDto> </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no departments are found.
    /// </exception>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(string? search = null)
    {
        IEnumerable<DepartmentDto> result = await _departmentRepository.GetDepartmentsAsync(search);

        if (result == null || !result.Any())
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_DEPARTMENT"]]);
        }

        return result;
    }
    
    /// <summary> Get Department By Id </summary>
    /// <param name="departmentId"></param>
    /// <returns> DepartmentDto </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the department with the specified ID is not found.
    /// </exception>
    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int departmentId)
    {
        DepartmentDto? result = await _departmentRepository.GetDepartmentByIdAsync(departmentId);

        if (result == null)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_DEPARTMENT"]]);
        }

        return result;
    }
    
    /// <summary> Add Department </summary>
    /// <param name="departmentCreateDto"></param>
    /// <returns> DepartmentDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when a department with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the department.
    /// </exception>
    public async Task<DepartmentDto> AddDepartmentAsync(DepartmentCreateDto departmentCreateDto)
    {
        DepartmentDto departmentDto = _mapper.Map<DepartmentDto>(departmentCreateDto);

        DepartmentResponseDto result = await _departmentRepository.AddUpdateDepartmentAsync(departmentDto);

        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_DEPARTMENT_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        departmentDto.Id = result.ResultId;
        return departmentDto;
    }
    
    /// <summary> Update Department </summary>
    /// <param name="departmentUpdateDto"></param>
    /// <returns> DepartmentDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when a department with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while updating the department.
    /// </exception>
    public async Task<DepartmentDto> UpdateDepartmentAsync(DepartmentUpdateDto departmentUpdateDto)
    {
        DepartmentDto departmentDto = _mapper.Map<DepartmentDto>(departmentUpdateDto);
        DepartmentResponseDto result = await _departmentRepository.AddUpdateDepartmentAsync(departmentDto);
        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_DEPARTMENT_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
         departmentDto.Id = result.ResultId;
        return departmentDto;
    }
    
    /// <summary> Delete Department </summary>
    /// <param name="departmentId"></param>
    /// <param name="updatedBy"></param>
    /// <returns> DepartmentResponseDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the departmentId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the department with the specified ID is not found.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while deleting the department.
    /// </exception>
    public async Task<DepartmentResponseDto> DeleteDepartmentAsync(int departmentId, int updatedBy)
    {
        if (departmentId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_DEPARTMENT"]]);
        }

        DepartmentResponseDto result = await _departmentRepository.DeleteDepartmentAsync(departmentId, updatedBy);

        if (result.ReturnValue == (int)StatusCode.NotFound)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_DEPARTMENT"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return result;
    }
}
