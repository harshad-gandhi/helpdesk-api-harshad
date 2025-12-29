using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto> AddCategoryAsync(CategoryCreateDto category);
    Task<CategoryDto> UpdateCategoryAsync(CategoryUpdateDto category);
    Task<KnowledgeBaseResponseDto> DeleteCategoryAsync(int categoryId, int updatedBy);
    Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? projectId = null,string? search = null);
      
   
    
}
