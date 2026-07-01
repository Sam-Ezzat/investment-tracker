using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InvestmentTracker.Application.Interfaces;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Controller for generating and downloading reports
/// </summary>
public class ReportController : Controller
{
    private readonly IReportService _reportService;
    private readonly IParticipantService _participantService;

    public ReportController(IReportService reportService, IParticipantService participantService)
    {
        _reportService = reportService;
        _participantService = participantService;
    }

    // GET: Report
    public async Task<IActionResult> Index()
    {
        // Load participants for dropdown
        var participants = await _participantService.GetAllAsync();
        ViewBag.Participants = new SelectList(participants, "Id", "FullName");
        
        // Populate year and month dropdowns
        var currentYear = DateTime.Now.Year;
        ViewBag.Years = Enumerable.Range(currentYear - 5, 10).Select(y => new SelectListItem { Value = y.ToString(), Text = y.ToString() });
        ViewBag.Months = Enumerable.Range(1, 12).Select(m => new SelectListItem { Value = m.ToString(), Text = new DateTime(2000, m, 1).ToString("MMMM") });

        return View();
    }

    #region Participant Statement

    // POST: Report/ParticipantStatementExcel
    [HttpPost]
    public async Task<IActionResult> ParticipantStatementExcel(int participantId, DateTime? startDate, DateTime? endDate)
    {
        try
        {
            DateOnly? start = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
            DateOnly? end = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

            var fileBytes = await _reportService.GenerateParticipantStatementExcelAsync(participantId, start, end);
            
            var participant = await _participantService.GetByIdAsync(participantId);
            var participantName = participant?.FullName?.Replace(" ", "_") ?? "Unknown";
            var fileName = $"ParticipantStatement_{participantName}_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Report/ParticipantStatementPdf
    [HttpPost]
    public async Task<IActionResult> ParticipantStatementPdf(int participantId, DateTime? startDate, DateTime? endDate)
    {
        try
        {
            DateOnly? start = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
            DateOnly? end = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

            var fileBytes = await _reportService.GenerateParticipantStatementPdfAsync(participantId, start, end);
            
            var participant = await _participantService.GetByIdAsync(participantId);
            var participantName = participant?.FullName?.Replace(" ", "_") ?? "Unknown";
            var fileName = $"ParticipantStatement_{participantName}_{DateTime.Now:yyyyMMdd}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Investment Summary

    // POST: Report/InvestmentSummaryExcel
    [HttpPost]
    public async Task<IActionResult> InvestmentSummaryExcel(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            DateOnly? start = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
            DateOnly? end = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

            var fileBytes = await _reportService.GenerateInvestmentSummaryExcelAsync(start, end);
            var fileName = $"InvestmentSummary_{DateTime.Now:yyyyMMdd}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Report/InvestmentSummaryPdf
    [HttpPost]
    public async Task<IActionResult> InvestmentSummaryPdf(DateTime? startDate, DateTime? endDate)
    {
        try
        {
            DateOnly? start = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
            DateOnly? end = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

            var fileBytes = await _reportService.GenerateInvestmentSummaryPdfAsync(start, end);
            var fileName = $"InvestmentSummary_{DateTime.Now:yyyyMMdd}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion

    #region Monthly Cash Flow

    // POST: Report/MonthlyCashFlowExcel
    [HttpPost]
    public async Task<IActionResult> MonthlyCashFlowExcel(int year, int month)
    {
        try
        {
            var fileBytes = await _reportService.GenerateMonthlyCashFlowExcelAsync(year, month);
            var monthName = new DateTime(year, month, 1).ToString("MMMM_yyyy");
            var fileName = $"MonthlyCashFlow_{monthName}.xlsx";
            
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: Report/MonthlyCashFlowPdf
    [HttpPost]
    public async Task<IActionResult> MonthlyCashFlowPdf(int year, int month)
    {
        try
        {
            var fileBytes = await _reportService.GenerateMonthlyCashFlowPdfAsync(year, month);
            var monthName = new DateTime(year, month, 1).ToString("MMMM_yyyy");
            var fileName = $"MonthlyCashFlow_{monthName}.pdf";
            
            return File(fileBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error generating report: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}
