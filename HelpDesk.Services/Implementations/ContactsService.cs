using AutoMapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Enums;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interface;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Services.Implementations
{
    public class ContactsService(IPersonsRepository personsRepository,
        IOrganizationsRepository organizationsRepository,
        IMapper mapper,
        IStringLocalizer<Messages> localizer) : IContactsService
    {
        private readonly IPersonsRepository _personsRepository = personsRepository;
        private readonly IOrganizationsRepository _organizationsRepository = organizationsRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<Messages> _localizer = localizer;

        #region Person

        /// <summary>
        /// Create Person
        /// </summary>
        /// <param name="personsCreateDto"></param>
        /// <returns>
        /// Returns the created <see cref="PersonsDto"/> object containing details of the newly created person. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<PersonsDto> CreatePersonAsync(PersonsCreateDto personsCreateDto)
        {
            PersonsDto personsDto = _mapper.Map<PersonsDto>(personsCreateDto);

            StoredProcedureResult<int> result = await _personsRepository.CreateOrUpdatePersonAsync(personsDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            personsDto.Id = result.Data;

            return personsDto;
        }

        /// <summary>
        /// Update Person
        /// </summary>
        /// <param name="personsDto"></param>
        /// <returns>
        /// Returns the updated <see cref="PersonsDto"/> object containing details of the updated person. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<PersonsDto> UpdatePersonAsync(PersonsDto personsDto)
        {
            StoredProcedureResult<int> result = await _personsRepository.CreateOrUpdatePersonAsync(personsDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.PersonDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_PERSON"]));
                }
            }

            personsDto.Id = result.Data;

            return personsDto;
        }

        /// <summary>
        /// Delete Person
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIdStr"></param>
        /// <returns>
        /// Returns a boolean value indicating whether the person was successfully deleted. 
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="BadRequestException"></exception>
        public async Task<bool> DeletePersonAsync(int id, string userIdStr)
        {
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedBy))
                throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

            if (id <= 0)
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_PERSON_ID"]));

            StoredProcedureResult<bool> result = await _personsRepository.DeletePersonAsync(id, updatedBy);
            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.PersonDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_PERSON"]));
                }
                else if (result.ReturnValue == (int)StatusCode.PersonHasActiveTicket)
                {
                    throw new BadRequestException(string.Format(_localizer["PERSON_HAS_ACTIVE_TICKET_CANNOT_DELETE"]));
                }
                else if (result.ReturnValue == (int)StatusCode.PersonHasActiveChat)
                {
                    throw new BadRequestException(string.Format(_localizer["PERSON_HAS_ACTIVE_CHAT_CANNOT_DELETE"]));
                }
            }

            return true;
        }

        /// <summary>
        /// Get Person By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="PersonsDto"/> object containing details of the person with the specified ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<PersonsDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_PERSON_ID"]));
            }

            PersonsDto? personsDto = await _personsRepository.GetPersonByIdAsync(id);

            if (personsDto == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_PERSON"]));
            }

            return personsDto;
        }

        /// <summary>
        /// Get Persons By Filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>
        /// Returns a <see cref="PagedResult{PersonsListDto}"/> object containing a paginated list of persons that match the specified filter criteria. 
        /// </returns>
        public async Task<PagedResult<PersonsListDto>> GetPersonsByFilterAsync(PersonFilterRequestDto filter)
        {
            PagedResult<PersonsListDto> result = await _personsRepository.GetPersonsByFiltersAsync(filter);
            return result;
        }

        /// <summary>
        /// Get Persons For Dropdown
        /// </summary>
        /// <param name="personDropdownRequestDto"></param>
        /// <returns>
        /// Returns a list of <see cref="PersonDropdownResponseDto"/> objects suitable for populating a dropdown menu based on the specified filter criteria. 
        /// </returns>
        public async Task<List<PersonDropdownResponseDto>> GetPersonsForDropdownAsync(PersonDropdownRequestDto personDropdownRequestDto)
        {
            PersonFilterRequestDto filter = _mapper.Map<PersonFilterRequestDto>(personDropdownRequestDto);
            PagedResult<PersonsListDto> result = await _personsRepository.GetPersonsByFiltersAsync(filter);
            List<PersonDropdownResponseDto> personDropdownResponseDto = _mapper.Map<List<PersonDropdownResponseDto>>(result.Items);
            return personDropdownResponseDto;
        }
        #endregion

        #region  Organization

        /// <summary>
        /// Create Organization
        /// </summary>
        /// <param name="organizationsCreateDto"></param>
        /// <returns>
        /// Returns the created <see cref="OrganizationsDto"/> object containing details of the newly created organization. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        public async Task<OrganizationsDto> CreateOrganizationAsync(OrganizationsCreateDto organizationsCreateDto)
        {
            OrganizationsDto organizationDto = _mapper.Map<OrganizationsDto>(organizationsCreateDto);

            StoredProcedureResult<int> result = await _organizationsRepository.CreateOrUpdateOrganizationAsync(organizationDto);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
            }

            organizationDto.Id = result.Data;
            return organizationDto;
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="organizationDto"></param>
        /// <returns>
        /// Returns the updated <see cref="OrganizationsDto"/> object containing details of the updated organization. 
        /// </returns>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<OrganizationsDto> UpdateOrganizationAsync(OrganizationsDto organizationDto)
        {
            StoredProcedureResult<int> result = await _organizationsRepository.CreateOrUpdateOrganizationAsync(organizationDto);
            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.OrganizationDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_ORGANIZATION"]));
                }
            }

            organizationDto.Id = result.Data;
            return organizationDto;
        }

        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userIdStr"></param>
        /// <returns>
        /// Returns a boolean value indicating whether the organization was successfully deleted. 
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<bool> DeleteOrganizationAsync(int id, string userIdStr)
        {
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int updatedBy))
                throw new UnauthorizedAccessException("INVALID_OR_MISSING_USER_ID");

            if (id <= 0)
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_ORGANIZATION_ID"]));

            StoredProcedureResult<bool> result = await _organizationsRepository.DeleteOrganizationAsync(id, updatedBy);

            if (!(result.ReturnValue > 0))
            {
                if (result.ReturnValue == (int)StatusCode.InternalServerError)
                {
                    throw new InternalServerErrorException(_localizer["INTERNAL_SERVER"]);
                }
                else if (result.ReturnValue == (int)StatusCode.OrganizationDataNotFound)
                {
                    throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_ORGANIZATION"]));
                }
            }

            return true;
        }

        /// <summary>
        /// Get Organization By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns the <see cref="OrganizationsDto"/> object containing details of the organization with the specified ID. 
        /// </returns>
        /// <exception cref="ValidationException"></exception>
        /// <exception cref="NotFoundException"></exception>
        public async Task<OrganizationsDto> GetOrganizationByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(string.Format(_localizer["PARAMETER_GREATER_THAN_ZERO"], _localizer["FIELD_ORGANIZATION_ID"]));
            }

            OrganizationsDto? organizationsDto = await _organizationsRepository.GetOrganizationByIdAsync(id);

            if (organizationsDto == null)
            {
                throw new NotFoundException(string.Format(_localizer["DATA_NOT_EXISTS"], _localizer["FIELD_ORGANIZATION"]));
            }

            return organizationsDto;
        }

        /// <summary>
        /// Get Organizations By Filter
        /// </summary>
        /// <param name="organizationFilterRequestDto"></param>
        /// <returns>
        /// Returns a <see cref="PagedResult{OrganizationListDto}"/> object containing a paginated list of organizations that match the specified filter criteria. 
        /// </returns>
        public async Task<PagedResult<OrganizationListDto>> GetOrganizationsByFilterAsync(OrganizationFilterRequestDto organizationFilterRequestDto)
        {
            PagedResult<OrganizationListDto> result = await _organizationsRepository.GetOrganizationsByFilterAsync(organizationFilterRequestDto);
            return result;
        }
        
        /// <summary>
        /// Get Organizations For Dropdown
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>
        /// Returns a list of <see cref="OrganizationDropdownResponseDto"/> objects suitable for populating a dropdown menu based on the specified project ID. 
        /// </returns>
        public async Task<List<OrganizationDropdownResponseDto>> GetOrganizationsForDropdownAsync(int ProjectId)
        {
            OrganizationFilterRequestDto filter = new()
            {
                ProjectId = ProjectId,

            };
            PagedResult<OrganizationListDto> result = await _organizationsRepository.GetOrganizationsByFilterAsync(filter);
            List<OrganizationDropdownResponseDto> organizationDropdownResponseDto = _mapper.Map<List<OrganizationDropdownResponseDto>>(result.Items);
            return organizationDropdownResponseDto;
        }
        #endregion
    }
}

