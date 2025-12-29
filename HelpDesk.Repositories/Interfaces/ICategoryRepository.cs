using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<KnowledgeBaseResponseDto> AddUpdateCategoryAsync(CategoryDto category);
    Task<KnowledgeBaseResponseDto> DeleteCategoryAsync(int categoryId, int UpdatedBy);
    Task<CategoryDto?> GetCategoryByIdAsync(int categoryId);
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? projectId = null, string? search = null);


}
