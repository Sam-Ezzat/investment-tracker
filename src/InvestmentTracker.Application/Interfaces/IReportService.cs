namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for generating reports
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Generate participant statement in Excel format
    /// </summary>
    Task<byte[]> GenerateParticipantStatementExcelAsync(int participantId, DateOnly? startDate = null, DateOnly? endDate = null);

    /// <summary>
    /// Generate participant statement in PDF format
    /// </summary>
    Task<byte[]> GenerateParticipantStatementPdfAsync(int participantId, DateOnly? startDate = null, DateOnly? endDate = null);

    /// <summary>
    /// Generate investment summary in Excel format
    /// </summary>
    Task<byte[]> GenerateInvestmentSummaryExcelAsync(DateOnly? startDate = null, DateOnly? endDate = null);

    /// <summary>
    /// Generate investment summary in PDF format
    /// </summary>
    Task<byte[]> GenerateInvestmentSummaryPdfAsync(DateOnly? startDate = null, DateOnly? endDate = null);

    /// <summary>
    /// Generate monthly cash flow report in Excel format
    /// </summary>
    Task<byte[]> GenerateMonthlyCashFlowExcelAsync(int year, int month);

    /// <summary>
    /// Generate monthly cash flow report in PDF format
    /// </summary>
    Task<byte[]> GenerateMonthlyCashFlowPdfAsync(int year, int month);
}
