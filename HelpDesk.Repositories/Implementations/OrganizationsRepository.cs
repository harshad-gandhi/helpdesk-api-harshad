using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interface;
using HelpDesk.Repositories.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HelpDesk.Repositories.Implementations
{
    public class OrganizationsRepository(IDbConnectionFactory connectionFactory) : IOrganizationsRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateOrUpdateOrganizationAsync
        public async Task<StoredProcedureResult<int>> CreateOrUpdateOrganizationAsync(OrganizationsDto organizationsDto)
        {
            const string spName = "usp_organizations_save";

            DynamicParameters parameters = new();

            parameters.Add("@Id", organizationsDto.Id);
            parameters.Add("@ProjectId", organizationsDto.ProjectId);
            parameters.Add("@Name", organizationsDto.Name);
            parameters.Add("@Email", organizationsDto.Email);
            parameters.Add("@Phone", organizationsDto.Phone);
            parameters.Add("@IsDeleted", organizationsDto.IsDeleted);
            parameters.Add("@CreatedBy", organizationsDto.CreatedBy);
            parameters.Add("@CreatedAt", organizationsDto.CreatedAt);
            parameters.Add("@UpdatedBy", organizationsDto.UpdatedBy);
            parameters.Add("@UpdatedAt", organizationsDto.UpdatedAt);

            parameters.Add("@ResultId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new StoredProcedureResult<int>
            {
                Data = parameters.Get<int>("@ResultId"),
                ReturnValue = parameters.Get<int>("@ReturnValue")
            };
        }
        #endregion

        #region DeleteOrganizationAsync
        public async Task<StoredProcedureResult<bool>> DeleteOrganizationAsync(int id, int updatedBy)
        {
            const string spName = "usp_organizations_delete";

            DynamicParameters parameters = new();

            parameters.Add("@Id", id);
            parameters.Add("@UpdatedBy", updatedBy);
            parameters.Add("@ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await _baseRepository.ExecuteAsync(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
            int returnValue = parameters.Get<int>("@ReturnValue");
            return new StoredProcedureResult<bool>
            {
                Data = returnValue == (int)StatusCode.DeletedSuccessfully,
                ReturnValue = returnValue
            };
        }
        #endregion

        #region GetOrganizationByIdAsync
        public async Task<OrganizationsDto?> GetOrganizationByIdAsync(int id)
        {
            const string spName = "usp_organizations_get_by_id";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id);

            OrganizationsDto? organizationsDto = await _baseRepository.QueryFirstOrDefaultAsync<OrganizationsDto>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return organizationsDto;
        }
        #endregion

        #region GetOrganizationsByFilterAsync
        public async Task<PagedResult<OrganizationListDto>> GetOrganizationsByFilterAsync(OrganizationFilterRequestDto organizationFilterRequestDto)
        {
            const string spName = "usp_organizations_get_by_filters";

            DynamicParameters parameters = new();

            parameters.Add("@PageNumber", organizationFilterRequestDto.PageNumber);
            parameters.Add("@PageSize", organizationFilterRequestDto.PageSize);
            parameters.Add("@Search", organizationFilterRequestDto.Search);
            parameters.Add("@SortBy", organizationFilterRequestDto.SortBy);
            parameters.Add("@SortDirection", organizationFilterRequestDto.SortDirection);
            parameters.Add("@ProjectId", organizationFilterRequestDto.ProjectId);

            var result = await _baseRepository.QueryMultipleAsync(
                spName,
                parameters,
                async multi =>
                {
                    var items = (await multi.ReadAsync<OrganizationListDto>()).AsList();
                    var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                    return new PagedResult<OrganizationListDto>
                    {
                        Items = items,
                        TotalCount = totalCount
                    };
                },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
        #endregion

    }

}

