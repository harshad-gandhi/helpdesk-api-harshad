using System.Net;
using HelpDesk.Common.Constants;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HelpDesk.API.Controllers
{
    // [Authorize]
    [ApiController]
    [Route(SystemConstant.API_REPORTING)]
    [Produces(SystemConstant.APPLICATION_JSON)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ReportingController(IWebHostEnvironment environment, IReportingService reportingService, IResponseService<object> responseService, IStringLocalizer<Messages> localizer) : ControllerBase
    {
        private readonly IReportingService _reportingService = reportingService;

        private readonly IResponseService<object> _responseService = responseService;

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        private readonly IWebHostEnvironment _environment = environment;

        /// <summary>
        /// Retrieves chat reporting data including total conversations, customer satisfaction ratings,
        /// and chat volume for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria containing the start and end date for the reporting period.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> with a <see cref="CombinedReportingDTO"/> containing
        /// total chats, customer satisfaction scores, and chat volume trends.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is missing or invalid.
        /// </exception>
        [HttpGet("chats-summary")]
        public async Task<IActionResult> GetTotalConversations([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            ReportingChatResultDTO reportingChatResultDTO = await _reportingService.GetTotalConversationsAsync(reportingRequestChatDTO);

            CustomerSatisfactionRatingsDTO customerSatisfactionRatingsDTO = await _reportingService.GetCustomerSatisfactionRatingsAsync(reportingRequestChatDTO);

            List<ChatVolumeDTO> chatVolumeDTOs = await _reportingService.GetChatVolumeAsync(reportingRequestChatDTO);

            CombinedReportingDTO combinedReportingDTO = new()
            {
                ReportingChatResultDTO = reportingChatResultDTO,
                CustomerSatisfactionRatingsDTO = customerSatisfactionRatingsDTO,
                ChatVolumeDTOs = chatVolumeDTOs
            };


            return _responseService.GetSuccessResponse(HttpStatusCode.OK, combinedReportingDTO);
        }

        /// <summary>
        /// Generates and downloads an Excel report containing chat history and analytics
        /// for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria including start and end date for exporting the chat report.
        /// </param>
        /// <returns>
        /// Returns a downloadable Excel file containing chat report data.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is not provided.
        /// </exception>
        [HttpGet("chat-excel")]
        public async Task<IActionResult> DownloadChatExcel([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            string webRootPath = _environment.WebRootPath;

            FileResult fileResult = await _reportingService.ExportChatReportToExcel(reportingRequestChatDTO, webRootPath);

            return fileResult;
        }


        /// <summary>
        /// Retrieves knowledge base reporting data including total article views, satisfaction ratings,
        /// and view and search trend for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria containing the start and end date for the reporting period.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> with a <see cref="CombinedKBReportingDTO"/> containing
        /// total article views,rating scores, and view and search trends.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is missing or invalid.
        /// </exception>
        [HttpGet("knowledgeBase-summary")]
        public async Task<IActionResult> GetTotalKBDataAsync([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            ReportingKBResultDTO reportingKBResultDTO = await _reportingService.GetTotalKBDataAsync(reportingRequestChatDTO);

            List<KBContentDistributionRatingDTO> kBContentDistributionRatingDTOs = await _reportingService.GetContentDistributionChartAsync(reportingRequestChatDTO);

            List<KBTrendsDTO> kBTrendsDTOs = await _reportingService.GetKBTrendsAsync(reportingRequestChatDTO);

            CombinedKBReportingDTO combinedKBReportingDTO = new()
            {
                ReportingKBResultDTO = reportingKBResultDTO,
                KBContentDistributionRatingDTO = kBContentDistributionRatingDTOs,
                KBTrendsDTOs = kBTrendsDTOs
            };

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, combinedKBReportingDTO);
        }

        /// <summary>
        /// Generates and downloads an Excel report containing ticket and analytics
        /// for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria including start and end date for exporting the chat report.
        /// </param>
        /// <returns>
        /// Returns a downloadable Excel file containing chat report data.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is not provided.
        /// </exception>
        [HttpGet("kb-excel")]
        public async Task<IActionResult> DownloadKBExcel([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            string webRootPath = _environment.WebRootPath;

            FileResult fileResult = await _reportingService.ExportKBReportToExcel(reportingRequestChatDTO, webRootPath);

            return fileResult;
        }

        /// <summary>
        /// Retrieves comprehensive ticket reporting data for the specified date range,
        /// including total tickets, resolved tickets, average resolution time, ticket trends,
        /// and priority distribution.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria containing the start and end dates for the reporting period.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IActionResult"/> containing a <see cref="CombinedTicketReportingDTO"/> 
        /// with overall ticket metrics, ticket trends, and priority distribution data.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is missing or invalid.
        /// </exception>
        [HttpGet("ticket-summary")]
        public async Task<IActionResult> GetTotalTicketDataAsync([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            ReportingTicketResultDTO reportingTicketResultDTO = await _reportingService.GetTotalTicketDataAsync(reportingRequestChatDTO);

            List<TicketTrendsDTO> ticketTrendsDTOs = await _reportingService.GetTicketTrendsAsync(reportingRequestChatDTO);

            List<TicketPriorityDistributionDTO> ticketPriorityDistributionDTOs = await _reportingService.GetPriorityDistributionChartAsync(reportingRequestChatDTO);

            CombinedTicketReportingDTO combinedTicketReportingDTO = new()
            {
                ReportingTicketResultDTO = reportingTicketResultDTO,
                TicketPriorityDistributionDTO = ticketPriorityDistributionDTOs,
                TicketTrendsDTOs = ticketTrendsDTOs
            };

            return _responseService.GetSuccessResponse(HttpStatusCode.OK, combinedTicketReportingDTO);
        }

        /// <summary>
        /// Generates and downloads an Excel report containing summarized ticket data
        /// and analytics for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// The filter criteria including start and end dates for generating the ticket report.
        /// </param>
        /// <returns>
        /// Returns a downloadable Excel file containing ticket summary, priority, and trend analytics.
        /// </returns>
        /// <exception cref="BadRequestException">
        /// Thrown when the start date or end date is not provided or invalid.
        /// </exception>
        [HttpGet("ticket-excel")]
        public async Task<IActionResult> DownloadTicketExcel([FromQuery] ReportingRequestChatDTO reportingRequestChatDTO)
        {
            if (reportingRequestChatDTO.StartDate == default || reportingRequestChatDTO.EndDate == default)
                throw new BadRequestException(string.Format(_localizer["REQUIRED"], "Date"));

            string webRootPath = _environment.WebRootPath;

            FileResult fileResult = await _reportingService.ExportTicketReportToExcel(reportingRequestChatDTO, webRootPath);

            return fileResult;
        }

    }
}