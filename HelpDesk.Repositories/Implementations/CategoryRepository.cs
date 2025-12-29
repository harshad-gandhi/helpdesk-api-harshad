using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations;

public class CategoryRepository(IDbConnectionFactory connectionFactory) : ICategoryRepository
{
    private readonly BaseRepository _baseRepository = new(connectionFactory);

    /// <summary>
    /// Adds a new category or updates an existing one in the knowledge base.
    /// </summary>
    /// <param name="category"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The newly created or updated category ID.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> AddUpdateCategoryAsync(CategoryDto category)
    {
        const string spName = "usp_category_save";

        DynamicParameters parameters = new();
        parameters.Add("Id", category.Id);
        parameters.Add("ProjectId", category.ProjectId);
        parameters.Add("ParentCategoryId", category.ParentCategoryId);
        parameters.Add("Name", category.Name);
        parameters.Add("Slug", category.Slug);
        parameters.Add("Description", category.Description);
        parameters.Add("IconUrl", category.IconUrl);
        if (category.Id == null || category.Id == 0)
        {
            parameters.Add("CreatedBy", category.CreatedBy);
        }
        else
        {
            parameters.Add("UpdatedBy", category.UpdatedBy);
        }
        parameters.Add("ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = parameters.Get<int>("ResultId"),
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
   
    /// <summary>
    /// Deletes a category from the knowledge base.
    /// </summary>
    /// <param name="category"></param>
    /// <returns>
    /// A <see cref="KnowledgeBaseResponseDto"/> containing:
    /// ResultId: The ID of the deleted category.
    /// ReturnValue: The return status from the stored procedure.
    /// </returns>
    public async Task<KnowledgeBaseResponseDto> DeleteCategoryAsync(int categoryId, int UpdatedBy)
    {
        const string spName = "usp_category_delete";

        DynamicParameters parameters = new();
        parameters.Add("Id", categoryId);
        parameters.Add("UpdatedBy", UpdatedBy);
        parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _baseRepository.ExecuteAsync(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return new KnowledgeBaseResponseDto
        {
            ResultId = categoryId,
            ReturnValue = parameters.Get<int>("@ReturnValue")
        };
    }
  
    /// <summary>
    /// Retrieves a category by its ID.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns>
    /// The <see cref="CategoryDto"/> if found; otherwise, null.
    /// </returns>
    public async Task<CategoryDto?> GetCategoryByIdAsync(int categoryId)
    {
        const string spName = "usp_category_get_by_id";

        DynamicParameters parameters = new();
        parameters.Add("Id", categoryId);

        CategoryDto? result = await _baseRepository.QueryFirstOrDefaultAsync<CategoryDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
    
    /// <summary>
    /// Retrieves a list of categories, optionally filtered by project ID and/or search term.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="search"></param>
    /// <returns>
    /// A collection of <see cref="CategoryDto"/> objects.
    /// </returns>
    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? projectId = null, string? search = null)
    {
        const string spName = "usp_category_get";

        DynamicParameters parameters = new();

        if (projectId.HasValue)
        {
            parameters.Add("ProjectId", projectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add("Search", search);
        }

        IEnumerable<CategoryDto> result = await _baseRepository.QueryAsync<CategoryDto>(
            spName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return result;
    }
}
