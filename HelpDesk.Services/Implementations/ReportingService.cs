using HelpDesk.Common.DTOs.RequestDTOs;
using HelpDesk.Common.DTOs.ResultDTOs;
using HelpDesk.Repositories.Interfaces;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using HelpDesk.Common.DTOs.CommonDTOs;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace HelpDesk.Services.Implementations
{
  public class ReportingService(IReportingRepository reportingRepository) : IReportingService
  {
    private readonly IReportingRepository _reportingRepository = reportingRepository;

    /// <summary>
    /// Retrieves the total number of conversations for the specified reporting period and project.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <returns>
    /// Returns a <see cref="ReportingChatResultDTO"/> containing total conversations,
    /// average response time, resolution rate, and related reporting metrics.
    /// </returns>
    public async Task<ReportingChatResultDTO> GetTotalConversationsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetTotalConversationsAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves customer satisfaction rating data for the specified reporting period and project.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <returns>
    /// Returns a <see cref="CustomerSatisfactionRatingsDTO"/> containing average rating,
    /// rating percentage distribution, and related satisfaction metrics.
    /// </returns>
    public async Task<CustomerSatisfactionRatingsDTO> GetCustomerSatisfactionRatingsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetCustomerSatisfactionRatingsAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves daily chat volume statistics for the specified reporting period and project.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="ChatVolumeDTO"/> containing daily chat totals,
    /// resolved chats, and resolution rate percentages.
    /// </returns>
    public async Task<List<ChatVolumeDTO>> GetChatVolumeAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetChatVolumeAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Generates and exports a detailed chat report in Excel format based on the
    /// specified reporting criteria.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <param name="webRootPath">
    /// The root path of the web host, used for file resource access if needed.
    /// </param>
    /// <returns>
    /// Returns a <see cref="FileResult"/> representing an Excel report file containing
    /// chat summary metrics, customer satisfaction ratings, and chat volume breakdown.
    /// </returns>
    /// <remarks>
    /// The generated Excel sheet includes formatted cells, merged headers, date range,
    /// project name, summary metrics, and a tabular chat-volume breakdown.
    /// </remarks>
    public async Task<FileResult> ExportChatReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath)
    {
      ReportingChatResultDTO reportingChatResultDTO = await _reportingRepository.GetTotalConversationsAsync(reportingRequestChatDTO);
      CustomerSatisfactionRatingsDTO customerSatisfactionRatingsDTO = await _reportingRepository.GetCustomerSatisfactionRatingsAsync(reportingRequestChatDTO);
      List<ChatVolumeDTO> chatVolumeDTOs = await _reportingRepository.GetChatVolumeAsync(reportingRequestChatDTO);
      if (reportingRequestChatDTO.ProjectId > 0)
      {
        _ = reportingChatResultDTO.ProjectName ?? "All Project";
      }


      using XLWorkbook workbook = new();
      IXLWorksheet ws = workbook.Worksheets.Add("Chat Report");

      IXLRange titleRange = ws.Range("A1:M6").Merge();
      titleRange.Value = "Help Desk Chat Report";
      titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      titleRange.Style.Font.Bold = true;
      titleRange.Style.Font.FontSize = 22;
      titleRange.Style.Fill.BackgroundColor = XLColor.White;

      // Date Range And Project ID
      IXLRange dateLabel = ws.Range("A8:B9");
      dateLabel.Merge().Value = "Date Range:";
      dateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      dateLabel.Style.Font.Bold = true;
      dateLabel.Style.Font.FontColor = XLColor.White;
      dateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange dateValue = ws.Range("C8:F9");
      dateValue.Merge().Value = $"{reportingRequestChatDTO.StartDate:dd-MM-yyyy} to {reportingRequestChatDTO.EndDate:dd-MM-yyyy}";
      dateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectLabel = ws.Range("H8:I9");
      projectLabel.Merge().Value = "Project :";
      projectLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      projectLabel.Style.Font.FontColor = XLColor.White;
      projectLabel.Style.Font.Bold = true;
      projectLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectValue = ws.Range("J8:M9");
      projectValue.Merge().Value = reportingChatResultDTO.ProjectName ?? "All Projects";
      projectValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Total Con. and Avg. Rating
      IXLRange conversationLabel = ws.Range("A11:B12");
      conversationLabel.Merge().Value = "Total Conv. :";
      conversationLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      conversationLabel.Style.Font.FontColor = XLColor.White;
      conversationLabel.Style.Font.Bold = true;
      conversationLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange conversationValue = ws.Range("C11:F12");
      conversationValue.Merge().Value = reportingChatResultDTO.TotalConversations;
      conversationValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satLabel = ws.Range("H11:I12");
      satLabel.Merge().Value = "Avg Rating:";
      satLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satLabel.Style.Font.FontColor = XLColor.White;
      satLabel.Style.Font.Bold = true;
      satLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satValue = ws.Range("J11:M12");
      satValue.Merge().Value = customerSatisfactionRatingsDTO.AveragePercentage.ToString("0.00") ?? "0";
      satValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      // Satisfaction score and Resolution Rate
      IXLRange satisfactionLabel = ws.Range("A14:B15");
      satisfactionLabel.Merge().Value = "Satisfaction Score:";
      satisfactionLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satisfactionLabel.Style.Font.FontColor = XLColor.White;
      satisfactionLabel.Style.Font.Bold = true;
      satisfactionLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satisfactionValue = ws.Range("C14:F15");
      satisfactionValue.Merge().Value = reportingChatResultDTO.AvgResponseTimeSeconds;
      satisfactionValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateLabel = ws.Range("H14:I15");
      resolutionRateLabel.Merge().Value = "Resolution Rate:";
      resolutionRateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      resolutionRateLabel.Style.Font.FontColor = XLColor.White;
      resolutionRateLabel.Style.Font.Bold = true;
      resolutionRateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateValue = ws.Range("J14:M15");
      resolutionRateValue.Merge().Value = reportingChatResultDTO.ResolutionRatePercentage.ToString("0.00") ?? "0";
      resolutionRateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Style summary box
      ws.Range("A1:M6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range("A1:M6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      ws.Range("A1:M6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


      // ================== Table Header ==================
      ws.Range("A17:D18").Merge().Value = "Date";
      ws.Range("E17:G18").Merge().Value = "Total Chats";
      ws.Range("H17:J18").Merge().Value = "Resolved Chats";
      ws.Range("K17:M18").Merge().Value = "Resolution Rate";

      var header = ws.Range("A17:M18");
      header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      header.Style.Font.FontColor = XLColor.White;
      header.Style.Font.Bold = true;
      header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

      // // ================== Table Data ===================
      int startRow = 19;
      int r = startRow;

      foreach (var item in chatVolumeDTOs)
      {
        ws.Range(r, 1, r, 4).Merge().Value = item.Chat_Date.ToString("dd-MM-yyyy");
        ws.Range(r, 5, r, 7).Merge().Value = item.Total_Chats;
        ws.Range(r, 8, r, 10).Merge().Value = item.Resolved_Chats;

        double rate = item.Total_Chats > 0
            ? (double)item.Resolved_Chats / item.Total_Chats * 100
            : 0;

        ws.Range(r, 11, r, 13).Merge().Value = rate.ToString("0.00") + "%";

        r++;
      }

      ws.Range($"A{startRow}:M{r - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

      ws.Columns().AdjustToContents();

      // ================== Return File ==================
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      var bytes = stream.ToArray();

      return new FileContentResult(bytes,
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        FileDownloadName = $"ChatReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
      };
    }


    /// <summary>
    /// Generates and exports a detailed Knowledge Base (KB) report in Excel format
    /// based on the specified reporting criteria.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <param name="webRootPath">
    /// The root path of the web host, used for file resource access if needed.
    /// </param>
    /// <returns>
    /// Returns a <see cref="FileResult"/> representing an Excel file containing
    /// Knowledge Base metrics such as total article views, average rating,
    /// total searches, and view growth percentage.
    /// </returns>
    /// <remarks>
    /// The generated Excel report includes formatted headers, project details,
    /// summary statistics, and a detailed time-series table displaying article
    /// view and search ratio trends.
    /// </remarks>
    public async Task<FileResult> ExportKBReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath)
    {

      ReportingKBResultDTO reportingKBResultDTO = await _reportingRepository.GetTotalKBDataAsync(reportingRequestChatDTO);

      List<KBTrendsDTO> kBTrendsDTOs = await _reportingRepository.GetKBTrendsAsync(reportingRequestChatDTO);

      if (reportingRequestChatDTO.ProjectId > 0)
      {
        _ = reportingKBResultDTO.ProjectName ?? "All Project";
      }


      using XLWorkbook workbook = new();
      IXLWorksheet ws = workbook.Worksheets.Add("KB Report");

      IXLRange titleRange = ws.Range("A1:M6").Merge();
      titleRange.Value = "Help Desk KB Report";
      titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      titleRange.Style.Font.Bold = true;
      titleRange.Style.Font.FontSize = 22;
      titleRange.Style.Fill.BackgroundColor = XLColor.White;

      // Date Range And Project ID
      IXLRange dateLabel = ws.Range("A8:B9");
      dateLabel.Merge().Value = "Date Range:";
      dateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      dateLabel.Style.Font.Bold = true;
      dateLabel.Style.Font.FontColor = XLColor.White;
      dateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange dateValue = ws.Range("C8:F9");
      dateValue.Merge().Value = $"{reportingRequestChatDTO.StartDate:dd-MM-yyyy} to {reportingRequestChatDTO.EndDate:dd-MM-yyyy}";
      dateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectLabel = ws.Range("H8:I9");
      projectLabel.Merge().Value = "Project :";
      projectLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      projectLabel.Style.Font.FontColor = XLColor.White;
      projectLabel.Style.Font.Bold = true;
      projectLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectValue = ws.Range("J8:M9");
      projectValue.Merge().Value = reportingKBResultDTO.ProjectName ?? "All Projects";
      projectValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Total Con. and Avg. Rating
      IXLRange conversationLabel = ws.Range("A11:B12");
      conversationLabel.Merge().Value = "Total Article View. :";
      conversationLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      conversationLabel.Style.Font.FontColor = XLColor.White;
      conversationLabel.Style.Font.Bold = true;
      conversationLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange conversationValue = ws.Range("C11:F12");
      conversationValue.Merge().Value = reportingKBResultDTO.TotalArticleViews;
      conversationValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satLabel = ws.Range("H11:I12");
      satLabel.Merge().Value = "Avg Rating:";
      satLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satLabel.Style.Font.FontColor = XLColor.White;
      satLabel.Style.Font.Bold = true;
      satLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satValue = ws.Range("J11:M12");
      satValue.Merge().Value = reportingKBResultDTO.AvgRating.ToString("0.00") ?? "0";
      satValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      // Satisfaction score and Resolution Rate
      IXLRange satisfactionLabel = ws.Range("A14:B15");
      satisfactionLabel.Merge().Value = "Total Searches:";
      satisfactionLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satisfactionLabel.Style.Font.FontColor = XLColor.White;
      satisfactionLabel.Style.Font.Bold = true;
      satisfactionLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satisfactionValue = ws.Range("C14:F15");
      satisfactionValue.Merge().Value = reportingKBResultDTO.TotalSearches;
      satisfactionValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateLabel = ws.Range("H14:I15");
      resolutionRateLabel.Merge().Value = "View Growth:";
      resolutionRateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      resolutionRateLabel.Style.Font.FontColor = XLColor.White;
      resolutionRateLabel.Style.Font.Bold = true;
      resolutionRateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateValue = ws.Range("J14:M15");
      resolutionRateValue.Merge().Value = reportingKBResultDTO.ViewGrowthPercentage.ToString("0.00") ?? "0";
      resolutionRateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Style summary box
      ws.Range("A1:M6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range("A1:M6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      ws.Range("A1:M6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


      // ================== Table Header ==================
      ws.Range("A17:D18").Merge().Value = "Date";
      ws.Range("E17:G18").Merge().Value = "Total Views";
      ws.Range("H17:J18").Merge().Value = "Total Searches";
      ws.Range("K17:M18").Merge().Value = "View/Search Ratio";

      var header = ws.Range("A17:M18");
      header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      header.Style.Font.FontColor = XLColor.White;
      header.Style.Font.Bold = true;
      header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

      // // ================== Table Data ===================
      int startRow = 19;
      int r = startRow;

      foreach (var item in kBTrendsDTOs)
      {
        ws.Range(r, 1, r, 4).Merge().Value = item.Date.ToString("dd-MM-yyyy");
        ws.Range(r, 5, r, 7).Merge().Value = item.ArticleViews;
        ws.Range(r, 8, r, 10).Merge().Value = item.Searches;

        double ration = item.Searches > 0
            ? (double)item.ArticleViews / item.Searches
            : 0;

        ws.Range(r, 11, r, 13).Merge().Value = ration.ToString("0.00") + "%";

        r++;
      }

      ws.Range($"A{startRow}:M{r - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

      ws.Columns().AdjustToContents();

      // ================== Return File ==================
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      var bytes = stream.ToArray();

      return new FileContentResult(bytes,
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        FileDownloadName = $"KBReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
      };
    }

    /// <summary>
    /// Retrieves Knowledge Base (KB) summary statistics such as total article views,
    /// total searches, average rating, and view growth for the specified reporting period
    /// and project.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <returns>
    /// Returns a <see cref="reportingKBResultDTO"/> containing high-level
    /// Knowledge Base analytics including engagement and performance metrics.
    /// </returns>
    public async Task<ReportingKBResultDTO> GetTotalKBDataAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetTotalKBDataAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves Knowledge Base (KB) content distribution analytics showing how
    /// articles are rated or categorized based on user feedback or engagement metrics.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including project ID and optional date range filters.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="KBContentDistributionRatingDTO"/> representing
    /// rating-based or popularity-based article distribution within the KB.
    /// </returns>
    public async Task<List<KBContentDistributionRatingDTO>> GetContentDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetContentDistributionChartAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves Knowledge Base (KB) trend data showing how article views and
    /// searches vary over time within the specified date range.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including project ID, start date, and end date.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="KBTrendsDTO"/> representing time-based metrics
    /// such as daily article views, searches, and view-to-search ratios.
    /// </returns>
    public async Task<List<KBTrendsDTO>> GetKBTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetKBTrendsAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves aggregated ticket statistics such as total open, closed, and
    /// in-progress tickets for the specified reporting filters.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria such as project ID, start date, and end date.
    /// </param>
    /// <returns>
    /// Returns a <see cref="ReportingTicketResultDTO"/> object containing overall
    /// ticket metrics like total tickets, resolved count, and response rate.
    /// </returns>
    /// <remarks>
    /// This method is primarily used to populate summary-level statistics
    /// in the reporting dashboard.
    /// </remarks>
    public async Task<ReportingTicketResultDTO> GetTotalTicketDataAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetTotalTicketDataAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves time-based ticket trend data for visualizing ticket volume changes
    /// across the selected reporting period.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Includes filters such as project ID, start date, and end date to define
    /// the reporting range.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="TicketTrendsDTO"/> containing daily or monthly
    /// ticket trends including new, closed, and pending tickets.
    /// </returns>
    /// <remarks>
    /// Used for generating ticket trend line charts in reports or dashboards.
    /// </remarks>
    public async Task<List<TicketTrendsDTO>> GetTicketTrendsAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {
      return await _reportingRepository.GetTicketTrendsAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Retrieves data representing the distribution of open tickets by priority level.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including project ID and date range.
    /// </param>
    /// <returns>
    /// Returns a list of <see cref="TicketPriorityDistributionDTO"/> containing
    /// ticket counts for each priority level (e.g., Highest, High, Medium, Low, Lowest).
    /// </returns>
    /// <remarks>
    /// This data is typically used to populate a bar chart showing ticket distribution
    /// by priority in the reporting dashboard.
    /// </remarks>
    public async Task<List<TicketPriorityDistributionDTO>> GetPriorityDistributionChartAsync(ReportingRequestChatDTO reportingRequestChatDTO)
    {

      return await _reportingRepository.GetPriorityDistributionChartAsync(reportingRequestChatDTO);
    }

    /// <summary>
    /// Generates and exports a detailed Ticket  report in Excel format
    /// based on the specified reporting criteria.
    /// </summary>
    /// <param name="reportingRequestChatDTO">
    /// Contains filter criteria including start date, end date, and project ID.
    /// </param>
    /// <param name="webRootPath">
    /// The root path of the web host, used for file resource access if needed.
    /// </param>
    /// <returns>
    /// Returns a <see cref="FileResult"/> representing an Excel file containing
    /// Ticket   metrics such as total tickets open tickets, average response time etc.
    /// </returns>
    /// <remarks>
    /// The generated Excel report includes formatted headers, project details,
    /// summary statistics, and a detailed time-series table displaying article
    /// view and search ratio trends.
    /// </remarks>
    public async Task<FileResult> ExportTicketReportToExcel(ReportingRequestChatDTO reportingRequestChatDTO, string webRootPath)
    {


      ReportingTicketResultDTO reportingTicketResultDTO = await _reportingRepository.GetTotalTicketDataAsync(reportingRequestChatDTO);

      List<TicketTrendsDTO> ticketTrendsDTOs = await _reportingRepository.GetTicketTrendsAsync(reportingRequestChatDTO);

      if (reportingRequestChatDTO.ProjectId > 0)
      {
        _ = reportingTicketResultDTO.ProjectName ?? "All Project";
      }


      using XLWorkbook workbook = new();
      IXLWorksheet ws = workbook.Worksheets.Add("KB Report");

      IXLRange titleRange = ws.Range("A1:M6").Merge();
      titleRange.Value = "Help Desk Ticket Report";
      titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      titleRange.Style.Font.Bold = true;
      titleRange.Style.Font.FontSize = 22;
      titleRange.Style.Fill.BackgroundColor = XLColor.White;

      // Date Range And Project ID
      IXLRange dateLabel = ws.Range("A8:B9");
      dateLabel.Merge().Value = "Date Range:";
      dateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      dateLabel.Style.Font.Bold = true;
      dateLabel.Style.Font.FontColor = XLColor.White;
      dateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange dateValue = ws.Range("C8:F9");
      dateValue.Merge().Value = $"{reportingRequestChatDTO.StartDate:dd-MM-yyyy} to {reportingRequestChatDTO.EndDate:dd-MM-yyyy}";
      dateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      dateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      dateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectLabel = ws.Range("H8:I9");
      projectLabel.Merge().Value = "Project :";
      projectLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      projectLabel.Style.Font.FontColor = XLColor.White;
      projectLabel.Style.Font.Bold = true;
      projectLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange projectValue = ws.Range("J8:M9");
      projectValue.Merge().Value = reportingTicketResultDTO.ProjectName ?? "All Projects";
      projectValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      projectValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      projectValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Total Con. and Avg. Rating
      IXLRange conversationLabel = ws.Range("A11:B12");
      conversationLabel.Merge().Value = "Total Tickets. :";
      conversationLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      conversationLabel.Style.Font.FontColor = XLColor.White;
      conversationLabel.Style.Font.Bold = true;
      conversationLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange conversationValue = ws.Range("C11:F12");
      conversationValue.Merge().Value = reportingTicketResultDTO.TotalTickets;
      conversationValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      conversationValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      conversationValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satLabel = ws.Range("H11:I12");
      satLabel.Merge().Value = "Resolved Tickets:";
      satLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satLabel.Style.Font.FontColor = XLColor.White;
      satLabel.Style.Font.Bold = true;
      satLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satValue = ws.Range("J11:M12");
      satValue.Merge().Value = reportingTicketResultDTO.ResolvedTickets.ToString("0.00") ?? "0";
      satValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      // Satisfaction score and Resolution Rate
      IXLRange satisfactionLabel = ws.Range("A14:B15");
      satisfactionLabel.Merge().Value = "Open Ticket:";
      satisfactionLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      satisfactionLabel.Style.Font.FontColor = XLColor.White;
      satisfactionLabel.Style.Font.Bold = true;
      satisfactionLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange satisfactionValue = ws.Range("C14:F15");
      satisfactionValue.Merge().Value = reportingTicketResultDTO.OpenTickets;
      satisfactionValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      satisfactionValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      satisfactionValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateLabel = ws.Range("H14:I15");
      resolutionRateLabel.Merge().Value = "Avg Res.Time:";
      resolutionRateLabel.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      resolutionRateLabel.Style.Font.FontColor = XLColor.White;
      resolutionRateLabel.Style.Font.Bold = true;
      resolutionRateLabel.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateLabel.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateLabel.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      IXLRange resolutionRateValue = ws.Range("J14:M15");
      resolutionRateValue.Merge().Value = reportingTicketResultDTO.AvgResolutionTimeSeconds.ToString("0.00") ?? "0";
      resolutionRateValue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      resolutionRateValue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      resolutionRateValue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

      // Style summary box
      ws.Range("A1:M6").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range("A1:M6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      ws.Range("A1:M6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;


      // ================== Table Header ==================
      ws.Range("A17:D18").Merge().Value = "Date";
      ws.Range("E17:G18").Merge().Value = "Total Created";
      ws.Range("H17:J18").Merge().Value = "Total Resolved";
      ws.Range("K17:M18").Merge().Value = "Total Closed";

      var header = ws.Range("A17:M18");
      header.Style.Fill.BackgroundColor = XLColor.FromHtml("#1976d2");
      header.Style.Font.FontColor = XLColor.White;
      header.Style.Font.Bold = true;
      header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


      header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

      // // ================== Table Data ===================
      int startRow = 19;
      int r = startRow;

      foreach (var item in ticketTrendsDTOs)
      {
        ws.Range(r, 1, r, 4).Merge().Value = item.Date.ToString("dd-MM-yyyy");
        ws.Range(r, 5, r, 7).Merge().Value = item.Created;
        ws.Range(r, 8, r, 10).Merge().Value = item.Resolved;
        ws.Range(r, 11, r, 13).Merge().Value = item.Closed;
        r++;
      }

      ws.Range($"A{startRow}:M{r - 1}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
      ws.Range($"A{startRow}:M{r - 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

      ws.Columns().AdjustToContents();

      // ================== Return File ==================
      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      var bytes = stream.ToArray();

      return new FileContentResult(bytes,
          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
      {
        FileDownloadName = $"TicketReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx"
      };
    }
  }
}