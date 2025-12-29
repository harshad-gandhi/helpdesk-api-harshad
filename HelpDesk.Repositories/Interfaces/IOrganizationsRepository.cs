using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Repositories.Interface
{
    public interface IOrganizationsRepository
    {
        Task<StoredProcedureResult<int>> CreateOrUpdateOrganizationAsync(OrganizationsDto organizationsDto);
        Task<StoredProcedureResult<bool>> DeleteOrganizationAsync(int id, int updatedBy);
        Task<OrganizationsDto?> GetOrganizationByIdAsync(int id);
        Task<PagedResult<OrganizationListDto>> GetOrganizationsByFilterAsync(OrganizationFilterRequestDto organizationFilterRequestDto);
    }

}

