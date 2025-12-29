using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;

namespace HelpDesk.Services.Interfaces
{
    public interface IContactsService
    {
        #region Persons
        Task<PersonsDto> CreatePersonAsync(PersonsCreateDto personsCreateDto);
        Task<PersonsDto> UpdatePersonAsync(PersonsDto personsDto);
        Task<bool> DeletePersonAsync(int id, string userIdStr);
        Task<PersonsDto> GetPersonByIdAsync(int id);
        Task<PagedResult<PersonsListDto>> GetPersonsByFilterAsync(PersonFilterRequestDto filter);
        Task<List<PersonDropdownResponseDto>> GetPersonsForDropdownAsync(PersonDropdownRequestDto personDropdownRequestDto);
        #endregion

        #region Contacts
        Task<OrganizationsDto> CreateOrganizationAsync(OrganizationsCreateDto organizationsCreateDto);
        Task<OrganizationsDto> UpdateOrganizationAsync(OrganizationsDto organizationDto);
        Task<bool> DeleteOrganizationAsync(int id, string userIdStr);
        Task<OrganizationsDto> GetOrganizationByIdAsync(int id);
        Task<PagedResult<OrganizationListDto>> GetOrganizationsByFilterAsync(OrganizationFilterRequestDto organizationFilterRequestDto);
        Task<List<OrganizationDropdownResponseDto>> GetOrganizationsForDropdownAsync(int ProjectId);
        #endregion
    }

}

