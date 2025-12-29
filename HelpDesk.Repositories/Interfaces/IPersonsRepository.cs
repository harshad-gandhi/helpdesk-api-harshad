using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interfaces
{
    public interface IPersonsRepository
    {
        Task<StoredProcedureResult<int>> CreateOrUpdatePersonAsync(PersonsDto personsDto);
        Task<StoredProcedureResult<bool>> DeletePersonAsync(int id, int updatedBy);
        Task<PersonsDto?> GetPersonByIdAsync(int id);
        Task<PagedResult<PersonsListDto>> GetPersonsByFiltersAsync(PersonFilterRequestDto personFilterRequestDto);
    }

}

