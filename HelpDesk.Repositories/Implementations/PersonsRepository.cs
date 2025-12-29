using System.Data;
using Dapper;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Repositories.Interfaces;

namespace HelpDesk.Repositories.Implementations
{
    public class PersonsRepository(IDbConnectionFactory connectionFactory) : IPersonsRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        #region CreateOrUpdatePersonAsync
        public async Task<StoredProcedureResult<int>> CreateOrUpdatePersonAsync(PersonsDto personsDto)
        {
            const string spName = "usp_persons_save";

            var parameters = new DynamicParameters();

            parameters.Add("@Id", personsDto.Id);
            parameters.Add("@ProjectId", personsDto.ProjectId);
            parameters.Add("@OrganizationId", personsDto.OrganizationId);
            parameters.Add("@CountryId", personsDto.CountryId);
            parameters.Add("@FirstName", personsDto.FirstName);
            parameters.Add("@LastName", personsDto.LastName);
            parameters.Add("@Email", personsDto.Email);
            parameters.Add("@Phone", personsDto.Phone);
            parameters.Add("@City", personsDto.City);
            parameters.Add("@IsBlocked", personsDto.IsBlocked);
            parameters.Add("@IsDeleted", personsDto.IsDeleted);
            parameters.Add("@FirstChatAt", personsDto.FirstChatAt);
            parameters.Add("@LastSeenAt", personsDto.LastSeenAt);
            parameters.Add("@CreatedBy", personsDto.CreatedBy);
            parameters.Add("@CreatedAt", personsDto.CateatedAt);
            parameters.Add("@UpdatedBy", personsDto.UpdatedBy);
            parameters.Add("@UpdatedAt", personsDto.UpdatedAt);

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

        #region DeletePersonAsync
        public async Task<StoredProcedureResult<bool>> DeletePersonAsync(int id, int updatedBy)
        {
            const string spName = "usp_persons_delete";

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

        #region GetPersonByIdAsync
        public async Task<PersonsDto?> GetPersonByIdAsync(int id)
        {
            const string spName = "usp_persons_get_by_id";

            DynamicParameters parameters = new();
            parameters.Add("@Id", id);

            PersonsDto? personsDto = await _baseRepository.QueryFirstOrDefaultAsync<PersonsDto>(
                spName,
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return personsDto;
        }
        #endregion

        public async Task<PagedResult<PersonsListDto>> GetPersonsByFiltersAsync(PersonFilterRequestDto personFilterRequestDto)
        {
            const string spName = "usp_persons_get_by_filters";

            DynamicParameters parameters = new();

            parameters.Add("@PageNumber", personFilterRequestDto.PageNumber);
            parameters.Add("@PageSize", personFilterRequestDto.PageSize);
            parameters.Add("@Search", personFilterRequestDto.Search);
            parameters.Add("@SortBy", personFilterRequestDto.SortBy);
            parameters.Add("@SortDirection", personFilterRequestDto.SortDirection);
            parameters.Add("@IsBlocked", personFilterRequestDto.IsBlocked);
            parameters.Add("@ProjectId", personFilterRequestDto.ProjectId);

            var result = await _baseRepository.QueryMultipleAsync(
                spName,
                parameters,
                async multi =>
                {
                    var persons = multi.Read<PersonsListDto>().AsList();

                    var totalCount = await multi.ReadSingleAsync<int>();

                    return new PagedResult<PersonsListDto>
                    {
                        Items = persons,
                        TotalCount = totalCount
                    };
                },
                commandType: CommandType.StoredProcedure);

            return result;


        }


    }


}

