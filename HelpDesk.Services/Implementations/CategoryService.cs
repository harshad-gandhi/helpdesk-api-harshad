using AutoMapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations;

public class CategoryService(ICategoryRepository categoryRepository, IStringLocalizer<Messages> localizer, IMapper mapper) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IStringLocalizer<Messages> _localizer = localizer;
    private readonly IMapper _mapper = mapper;

    /// <summary> Add Category </summary>
    /// <param name="category"></param>
    /// <returns> CategoryDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when a category with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while adding the category.
    /// </exception>
    public async Task<CategoryDto> AddCategoryAsync(CategoryCreateDto category)
    {
        CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);

        KnowledgeBaseResponseDto result = await _categoryRepository.AddUpdateCategoryAsync(categoryDto);

        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_CATEGORY_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        categoryDto.Id = result.ResultId;
        return categoryDto;
    }
    
    /// <summary> Update Category </summary>
    /// <param name="category"></param>
    /// <returns> CategoryDto </returns>
    /// <exception cref="DataAlreadyExistsException">
    /// Thrown when a category with the same name already exists.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while updating the category.
    /// </exception>
    public async Task<CategoryDto> UpdateCategoryAsync(CategoryUpdateDto category)
    {
        CategoryDto categoryDto = _mapper.Map<CategoryDto>(category);
        KnowledgeBaseResponseDto result = await _categoryRepository.AddUpdateCategoryAsync(categoryDto);
        if (result.ReturnValue == (int)StatusCode.NameAlreadyExists)
        {
            throw new DataAlreadyExistsException(_localizer["DATA_ALREADY_EXIST", _localizer["FIELD_CATEGORY_NAME"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }
        categoryDto.Id = result.ResultId;
        return categoryDto;
    }

    /// <summary> Delete Category </summary>
    /// <param name="categoryId"></param>
    /// <param name="updatedBy"></param>
    /// <returns> KnowledgeBaseResponseDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the categoryId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the category to be deleted is not found.
    /// </exception>
    /// <exception cref="InternalServerErrorException">
    /// Thrown when there is an internal server error while deleting the category.
    /// </exception>
    public async Task<KnowledgeBaseResponseDto> DeleteCategoryAsync(int categoryId, int updatedBy)
    {
        if (categoryId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_CATEGORY"]]);
        }

        KnowledgeBaseResponseDto result = await _categoryRepository.DeleteCategoryAsync(categoryId, updatedBy);

        if (result.ReturnValue == (int)StatusCode.NotFound)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_CATEGORY"]]);
        }
        else if (result.ReturnValue == (int)StatusCode.InternalServerError)
        {
            throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
        }

        return result;
    }
    
    /// <summary> Get Category By Id </summary>
    /// <param name="categoryId"></param>
    /// <returns> CategoryDto </returns>
    /// <exception cref="ValidationException">
    /// Thrown when the categoryId is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the category with the specified ID is not found.
    /// </exception>
    public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
    {
        if (categoryId <= 0)
        {
            throw new ValidationException(_localizer["PARAMETER_GREATER_THAN_ZERO", _localizer["FIELD_CATEGORY"]]);
        }

        CategoryDto? result = await _categoryRepository.GetCategoryByIdAsync(categoryId);

        if (result == null)
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_CATEGORY"]]);
        }

        return result;
    }
   
    /// <summary> Get Categories </summary>
    /// <param name="projectId"></param>
    /// <param name="search"></param>
    /// <returns> IEnumerable<CategoryDto> </returns>
    /// <exception cref="NotFoundException">
    /// Thrown when no categories are found.
    /// </exception>
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? projectId = null, string? search = null)
    {

        IEnumerable<CategoryDto> result = await _categoryRepository.GetCategoriesAsync(projectId, search);

        if (result == null || !result.Any())
        {
            throw new NotFoundException(_localizer["DATA_NOT_FOUND", _localizer["FIELD_CATEGORY"]]);
        }

        return result;
    }
   
}
