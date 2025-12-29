using Microsoft.AspNetCore.Mvc;
using System.Net;
using HelpDesk.Common.DTOs.CommonDTOs;
using HelpDesk.Services.Interfaces;
using HelpDesk.Common.Constants;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route(SystemConstant.API_COUNTRIES)]
    [Produces(SystemConstant.APPLICATION_JSON)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class CountryController(IDirectMessageService directMessageService, IResponseService<object> responseService) : ControllerBase
    {

        private readonly IDirectMessageService _directMessageService = directMessageService;

        private readonly IResponseService<object> _responseService = responseService;

        /// <summary>
        /// Retrieves a list of all available countries.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a collection of <see cref="CountryDTO"/>
        /// objects wrapped in a success response with HTTP status code 200 (OK).
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCountriesAsync()
        {
            List<CountryDTO> countryDTOs = await _directMessageService.GetAllCountriesAsync();
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, countryDTOs);
        }

        /// <summary>
        /// Searches and retrieves a list of countries that match the provided keyword.
        /// </summary>
        /// <param name="keyword">
        /// The optional search term used to filter countries by name or related attributes.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a filtered list of <see cref="CountryDTO"/>
        /// objects wrapped in a success response with HTTP status code 200 (OK).
        /// </returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchCountriesAsync([FromQuery] string? keyword)
        {
            IEnumerable<CountryDTO> countryDTOs = await _directMessageService.SearchCountriesAsync(keyword);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, countryDTOs);
        }


        /// <summary>
        /// Get Country By Code
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing the CountryDTO for the specified country code.
        /// If the country is found, it returns a 200 OK response with the CountryDTO.
        /// </returns>
        [HttpGet("{countryCode}")]
        public async Task<IActionResult> GetCountryByCodeAsync([FromRoute] string countryCode)
        {
            CountryDTO countryDTO = await _directMessageService.GetCountryByCodeAsync(countryCode);
            return _responseService.GetSuccessResponse(HttpStatusCode.OK, countryDTO);
        }

    }
}