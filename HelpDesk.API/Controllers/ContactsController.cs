using System.Net;
using System.Security.Claims;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResponseDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ContactsController(IContactsService contactsService,
        IResponseService<object> responseService,
        IStringLocalizer<Messages> localizer) : ControllerBase
    {
        private readonly IContactsService _contactsService = contactsService;
        private readonly IResponseService<object> _responseService = responseService;
        private readonly IStringLocalizer<Messages> _localizer = localizer;

        #region Person

        /// <summary>
        /// Create Person
        /// </summary>
        /// <param name="personsCreateDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="PersonsDto"/>
        /// along with a success response and HTTP status code 200 (OK).        
        /// </returns>
        [HttpPost("persons/create")]
        public async Task<IActionResult> CreatePersonAsync([FromBody] PersonsCreateDto personsCreateDto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (userIdStr == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
            // personsCreateDto.CreatedBy = int.Parse(userIdStr);
            PersonsDto personsDto = await _contactsService.CreatePersonAsync(personsCreateDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.Created, personsDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_PERSON"]]]);
        }

        /// <summary>
        /// Update Person
        /// </summary>
        /// <param name="personsDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="PersonsDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpPost("persons/update")]
        public async Task<IActionResult> UpdatePersonAsync([FromBody] PersonsDto personsDto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
            personsDto.UpdatedBy = int.Parse(userIdStr);
            PersonsDto personsDtoResponse = await _contactsService.UpdatePersonAsync(personsDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, personsDtoResponse, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_PERSON"]]]);
        }

        /// <summary>
        /// Delete Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpDelete("persons/{id:int}")]
        public async Task<IActionResult> DeletePersonAsync(int id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isDeleted = await _contactsService.DeletePersonAsync(id, userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_PERSON"]]]);
        }

        /// <summary>
        /// Get Person By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="PersonsDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("persons/{id:int}")]
        public async Task<IActionResult> GetPersonByIdAsync(int id)
        {
            PersonsDto personsDto = await _contactsService.GetPersonByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, personsDto, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_PERSON"]]]);
        }

        /// <summary>
        /// Get Persons By Filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a paged result of <see cref="PersonsListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("persons/getbyfilter")]
        public async Task<IActionResult> GetPersonsByFilterAsync([FromQuery] PersonFilterRequestDto filter)
        {
            PagedResult<PersonsListDto> persons = await _contactsService.GetPersonsByFilterAsync(filter);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, persons, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_PERSONS"]]]);
        }

        /// <summary>
        /// Get Persons For Dropdown
        /// </summary>
        /// <param name="personDropdownRequestDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of <see cref="PersonDropdownResponseDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("persons/getdropdown")]
        public async Task<IActionResult> GetPersonsForDropdownAsync([FromQuery] PersonDropdownRequestDto personDropdownRequestDto)
        {
            List<PersonDropdownResponseDto> persons = await _contactsService.GetPersonsForDropdownAsync(personDropdownRequestDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, persons, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_PERSONS"]]]);
        }

        #endregion

        #region Organization

        /// <summary>
        /// Create Organization
        /// </summary>
        /// <param name="organizationsCreateDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the created <see cref="OrganizationsDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpPost("organizations/create")]
        public async Task<IActionResult> CreateOrganizationAsync([FromBody] OrganizationsCreateDto organizationsCreateDto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
            organizationsCreateDto.CreatedBy = int.Parse(userIdStr);
            OrganizationsDto organizationsDto = await _contactsService.CreateOrganizationAsync(organizationsCreateDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.Created, organizationsDto, [_localizer["ENTITY_CREATION_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        /// <summary>
        /// Update Organization
        /// </summary>
        /// <param name="organizationDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the updated <see cref="OrganizationsDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpPost("organizations/update")]
        public async Task<IActionResult> UpdateOrganizationAsync([FromBody] OrganizationsDto organizationDto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]);
            organizationDto.UpdatedBy = int.Parse(userIdStr);
            OrganizationsDto organizationsDtoResponse = await _contactsService.UpdateOrganizationAsync(organizationDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, organizationsDtoResponse, [_localizer["ENTITY_UPDATE_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        /// <summary>
        /// Delete Organization
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> indicating whether the deletion was successful,
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [HttpDelete("organizations/{id:int}")]
        public async Task<IActionResult> DeleteOrganizationAsync(int id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isDeleted = await _contactsService.DeleteOrganizationAsync(id, userIdStr ?? throw new UnauthorizedAccessException(_localizer["USER_ID_NOT_FOUND_IN_CLAIMS"]));
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, isDeleted, [_localizer["ENTITY_DELETION_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        /// <summary>
        /// Get Organization By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the retrieved <see cref="OrganizationsDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("organizations/{id:int}")]
        public async Task<IActionResult> GetOrganizationByIdAsync(int id)
        {
            OrganizationsDto organizationsDto = await _contactsService.GetOrganizationByIdAsync(id);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, organizationsDto, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        /// <summary>
        /// Get Organizations By Filter
        /// </summary>
        /// <param name="organizationFilterRequestDto"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a paged result of <see cref="OrganizationListDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("organizations/getbyfilter")]
        public async Task<IActionResult> GetOrganizationsByFilterAsync([FromQuery] OrganizationFilterRequestDto organizationFilterRequestDto)
        {
            PagedResult<OrganizationListDto> organizations = await _contactsService.GetOrganizationsByFilterAsync(organizationFilterRequestDto);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, organizations, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        /// <summary>
        /// Get Organizations For Dropdown
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a list of <see cref="OrganizationDropdownResponseDto"/>
        /// along with a success response and HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("organizations/getdropdown")]
        public async Task<IActionResult> GetOrganizationsForDropdownAsync([FromQuery] int ProjectId)
        {
            List<OrganizationDropdownResponseDto> organizations = await _contactsService.GetOrganizationsForDropdownAsync(ProjectId);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, organizations, [_localizer["ENTITY_RETRIEVAL_SUCCEED", _localizer["FIELD_ORGANIZATION"]]]);
        }

        #endregion
    }
}

