using System.Data;
using Dapper;
using HelpDesk.Common.CustomExceptions;
using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Common.Resources;
using HelpDesk.Repositories.Interfaces;
using Microsoft.Extensions.Localization;

namespace HelpDesk.Repositories.Implementations
{
    public class ReportingRepository(IDbConnectionFactory connectionFactory, IStringLocalizer<Messages> localizer) : IReportingRepository
    {
        private readonly BaseRepository _baseRepository = new(connectionFactory);

        private readonly IStringLocalizer<Messages> _localizer = localizer;

        /// <summary>
        /// Executes the reporting stored procedure to retrieve chat summary metrics such as
        /// total conversations, average response time, and resolution rate.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a <see cref="ReportingChatResultDTO"/> containing overall chat performance
        /// statistics for the specified filters.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure returns no data or encounters an execution error.
        /// </exception>
        public async Task<ReportingChatResultDTO> GetTotalConversationsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_chat_data";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);

            ReportingChatResultDTO? reportingResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ReportingChatResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);


            return reportingResultDTO;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve customer satisfaction rating
        /// metrics for the specified reporting period and project.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a <see cref="CustomerSatisfactionRatingsDTO"/> containing average rating,
        /// rating percentage distribution, and related satisfaction statistics.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure returns no data or encounters an execution error.
        /// </exception>
        public async Task<CustomerSatisfactionRatingsDTO> GetCustomerSatisfactionRatingsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_chat_data_chart";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);

            CustomerSatisfactionRatingsDTO? customerSatisfactionDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<CustomerSatisfactionRatingsDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);


            return customerSatisfactionDTO;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve chat volume trend data,
        /// grouped by date for the specified period and project.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="ChatVolumeDTO"/> representing daily chat totals,
        /// resolved chats, and resolution rate percentages.
        /// </returns>
        public async Task<List<ChatVolumeDTO>> GetChatVolumeAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_chat_data_graph";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);


            List<ChatVolumeDTO> customerSatisfactionDTO =
            [.. await _baseRepository.QueryAsync<ChatVolumeDTO>(spName, parameters, CommandType.StoredProcedure)];

            return customerSatisfactionDTO;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve Knowledge Base (KB)
        /// summary metrics such as total articles viewed, total search queries,
        /// and average engagement for the specified date range.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a <see cref="ReportingTicketResultDTO"/> containing high-level
        /// Knowledge Base analytics such as total views, search volume, and
        /// content performance statistics.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure returns no data or encounters an
        /// execution error during query processing.
        /// </exception>
        public async Task<ReportingKBResultDTO> GetTotalKBDataAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_kb_data";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);

            ReportingKBResultDTO reportingKBResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ReportingKBResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);


            return reportingKBResultDTO;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve Knowledge Base (KB)
        /// content distribution data, categorized by rating or popularity metrics.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID and optional date filters.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="KBContentDistributionRatingDTO"/> representing
        /// how articles are distributed across different rating categories such as
        /// Excellent, Good, Average, Poor, and Bad.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure fails or returns invalid data.
        /// </exception>
        public async Task<List<KBContentDistributionRatingDTO>> GetContentDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_kb_data_chart";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }

            List<KBContentDistributionRatingDTO> kBContentDistributionRatingDTOs =
                       [.. await _baseRepository.QueryAsync<KBContentDistributionRatingDTO>(spName, parameters, CommandType.StoredProcedure)];


            return kBContentDistributionRatingDTOs;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve Knowledge Base (KB)
        /// trend data over time, showing article views, search trends, or engagement
        /// growth within the specified reporting period.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="KBTrendsDTO"/> containing time-series data
        /// that represents Knowledge Base performance trends such as daily or
        /// weekly article views and search activity.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure encounters an error or produces no data.
        /// </exception>
        public async Task<List<KBTrendsDTO>> GetKBTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_kb_data_graph";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);


            List<KBTrendsDTO> kBTrendsDTOs =
            [.. await _baseRepository.QueryAsync<KBTrendsDTO>(spName, parameters, CommandType.StoredProcedure)];

            return kBTrendsDTOs;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve aggregated ticket summary metrics
        /// for the specified date range and optional project filter.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a <see cref="ReportingTicketResultDTO"/> containing high-level ticket analytics
        /// such as total tickets, resolved tickets, average resolution time (seconds),
        /// open tickets, and project name (if a project filter was provided).
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure returns no data or an execution error occurs.
        /// </exception>
        public async Task<ReportingTicketResultDTO> GetTotalTicketDataAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_ticket_data";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);

            ReportingTicketResultDTO reportingTicketResultDTO =
            await _baseRepository.QueryFirstOrDefaultAsync<ReportingTicketResultDTO>(spName, parameters, CommandType.StoredProcedure)
            ?? throw new InternalServerErrorException(_localizer["STORED_PROCEDURE_EXCEPTION_MESSAGE"]);


            return reportingTicketResultDTO;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve the distribution of tickets by priority
        /// for the specified date range and optional project filter.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="TicketPriorityDistributionDTO"/> where each entry represents
        /// a priority level and contains metrics such as total tickets, open tickets, and priority name.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure fails or returns invalid/empty data.
        /// </exception>
        public async Task<List<TicketPriorityDistributionDTO>> GetPriorityDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_ticket_data_chart";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);

            List<TicketPriorityDistributionDTO> ticketPriorityDistributionDTOs =
                       [.. await _baseRepository.QueryAsync<TicketPriorityDistributionDTO>(spName, parameters, CommandType.StoredProcedure)];


            return ticketPriorityDistributionDTOs;
        }

        /// <summary>
        /// Executes the reporting stored procedure to retrieve ticket trend data over time
        /// (for example daily or monthly counts) within the specified reporting period and project.
        /// </summary>
        /// <param name="reportingRequestChatDTO">
        /// Contains filter criteria including project ID, start date, and end date.
        /// </param>
        /// <returns>
        /// Returns a list of <see cref="TicketTrendsDTO"/> containing time-series ticket metrics
        /// such as date, new tickets, closed tickets, and open tickets for each time point.
        /// </returns>
        /// <exception cref="InternalServerErrorException">
        /// Thrown when the stored procedure encounters an error or returns no data.
        /// </exception>
        public async Task<List<TicketTrendsDTO>> GetTicketTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
        {
            const string spName = "usp_reporting_ticket_data_graph";

            DynamicParameters parameters = new();

            if (reportingRequestChatDTO.ProjectId > 0)
            {
                parameters.Add("@ProjectId", reportingRequestChatDTO.ProjectId);
            }
            parameters.Add("@StartDate", reportingRequestChatDTO.StartDate);
            parameters.Add("@EndDate", reportingRequestChatDTO.EndDate);


            List<TicketTrendsDTO> ticketTrendsDTOs =
            [.. await _baseRepository.QueryAsync<TicketTrendsDTO>(spName, parameters, CommandType.StoredProcedure)];

            return ticketTrendsDTOs;
        }
    }
}