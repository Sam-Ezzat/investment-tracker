using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Enums;
using InvestmentTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service for generating various reports in Excel and PDF formats
/// </summary>
public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ISettingService _settingService;

    public ReportService(ApplicationDbContext context, ISettingService settingService)
    {
        _context = context;
        _settingService = settingService;
        
        // Configure QuestPDF license
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Participant Statement

    public async Task<byte[]> GenerateParticipantStatementExcelAsync(int participantId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var participant = await _context.Participants
            .Include(p => p.InvestmentCycles)
                .ThenInclude(c => c.Payments)
            .FirstOrDefaultAsync(p => p.Id == participantId);

        if (participant == null)
            throw new ArgumentException("Participant not found");

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Participant Statement");

        // Header
        worksheet.Cell("A1").Value = companyName;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Font.Bold = true;

        worksheet.Cell("A2").Value = "Participant Statement";
        worksheet.Cell("A2").Style.Font.FontSize = 14;
        worksheet.Cell("A2").Style.Font.Bold = true;

        // Participant Info
        int row = 4;
        worksheet.Cell(row, 1).Value = "Participant Name:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = participant.FullName;
        row++;

        worksheet.Cell(row, 1).Value = "Email:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = participant.Email;
        row++;

        worksheet.Cell(row, 1).Value = "Phone:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = participant.Phone;
        row++;

        worksheet.Cell(row, 1).Value = "Report Date:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = DateTime.Now.ToString("MMMM dd, yyyy");
        row += 2;

        // Investment Cycles
        worksheet.Cell(row, 1).Value = "Investment Cycles";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        // Table Headers
        var headerRow = row;
        worksheet.Cell(row, 1).Value = "Cycle ID";
        worksheet.Cell(row, 2).Value = "Principal Amount";
        worksheet.Cell(row, 3).Value = "Start Date";
        worksheet.Cell(row, 4).Value = "End Date";
        worksheet.Cell(row, 5).Value = "Interest Rate";
        worksheet.Cell(row, 6).Value = "Expected Profit";
        worksheet.Cell(row, 7).Value = "Total Paid";
        worksheet.Cell(row, 8).Value = "Status";

        var headerRange = worksheet.Range(row, 1, row, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        row++;

        // Filter cycles by date if provided
        var cycles = participant.InvestmentCycles.AsQueryable();
        if (startDate.HasValue)
            cycles = cycles.Where(c => c.StartDate >= startDate.Value).AsQueryable();
        if (endDate.HasValue)
            cycles = cycles.Where(c => c.StartDate <= endDate.Value).AsQueryable();

        var cyclesList = cycles.ToList();
        decimal totalPrincipal = 0;
        decimal totalProfit = 0;
        decimal totalPaid = 0;

        foreach (var cycle in cyclesList)
        {
            worksheet.Cell(row, 1).Value = cycle.Id;
            worksheet.Cell(row, 2).Value = cycle.PrincipalAmount;
            worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 3).Value = cycle.StartDate.ToString("MMM dd, yyyy");
            worksheet.Cell(row, 4).Value = cycle.EndDate.ToString("MMM dd, yyyy");
            worksheet.Cell(row, 5).Value = cycle.InterestRate;
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.00\"%\"";
            worksheet.Cell(row, 6).Value = cycle.TotalExpectedProfit;
            worksheet.Cell(row, 6).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            
            var paid = cycle.Payments.Sum(p => p.Amount);
            worksheet.Cell(row, 7).Value = paid;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 8).Value = cycle.Status.ToString();

            totalPrincipal += cycle.PrincipalAmount;
            totalProfit += cycle.TotalExpectedProfit;
            totalPaid += paid;
            row++;
        }

        // Totals
        row++;
        worksheet.Cell(row, 1).Value = "TOTALS";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalPrincipal;
        worksheet.Cell(row, 2).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        worksheet.Cell(row, 6).Value = totalProfit;
        worksheet.Cell(row, 6).Style.Font.Bold = true;
        worksheet.Cell(row, 6).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        worksheet.Cell(row, 7).Value = totalPaid;
        worksheet.Cell(row, 7).Style.Font.Bold = true;
        worksheet.Cell(row, 7).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateParticipantStatementPdfAsync(int participantId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var participant = await _context.Participants
            .Include(p => p.InvestmentCycles)
                .ThenInclude(c => c.Payments)
            .FirstOrDefaultAsync(p => p.Id == participantId);

        if (participant == null)
            throw new ArgumentException("Participant not found");

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        // Filter cycles by date if provided
        var cycles = participant.InvestmentCycles.AsQueryable();
        if (startDate.HasValue)
            cycles = cycles.Where(c => c.StartDate >= startDate.Value).AsQueryable();
        if (endDate.HasValue)
            cycles = cycles.Where(c => c.StartDate <= endDate.Value).AsQueryable();

        var cyclesList = cycles.ToList();
        decimal totalPrincipal = cyclesList.Sum(c => c.PrincipalAmount);
        decimal totalProfit = cyclesList.Sum(c => c.TotalExpectedProfit);
        decimal totalPaid = cyclesList.Sum(c => c.Payments.Sum(p => p.Amount));

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(companyName).FontSize(18).Bold();
                            column.Item().Text("Participant Statement").FontSize(14).Bold();
                            column.Item().PaddingTop(5).Text($"Report Date: {DateTime.Now:MMMM dd, yyyy}").FontSize(9);
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Participant Info
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(text =>
                                {
                                    text.Span("Participant: ").Bold();
                                    text.Span(participant.FullName);
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Email: ").Bold();
                                    text.Span(participant.Email);
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Phone: ").Bold();
                                    text.Span(participant.Phone ?? "N/A");
                                });
                            });
                        });

                        column.Item().PaddingTop(10);

                        // Investment Cycles Table
                        column.Item().Text("Investment Cycles").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);  // ID
                                columns.RelativeColumn(1.5f);  // Principal
                                columns.RelativeColumn(1.2f);  // Start Date
                                columns.RelativeColumn(1.2f);  // End Date
                                columns.RelativeColumn(0.8f);  // Rate
                                columns.RelativeColumn(1.5f);  // Profit
                                columns.RelativeColumn(1.5f);  // Paid
                                columns.RelativeColumn(1);    // Status
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID").Bold();
                                header.Cell().Element(CellStyle).Text("Principal").Bold();
                                header.Cell().Element(CellStyle).Text("Start Date").Bold();
                                header.Cell().Element(CellStyle).Text("End Date").Bold();
                                header.Cell().Element(CellStyle).Text("Rate").Bold();
                                header.Cell().Element(CellStyle).Text("Profit").Bold();
                                header.Cell().Element(CellStyle).Text("Paid").Bold();
                                header.Cell().Element(CellStyle).Text("Status").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten2).Padding(5);
                                }
                            });

                            // Data
                            foreach (var cycle in cyclesList)
                            {
                                var paid = cycle.Payments.Sum(p => p.Amount);
                                table.Cell().Element(CellStyle).Text(cycle.Id.ToString());
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{cycle.PrincipalAmount:N2}");
                                table.Cell().Element(CellStyle).Text(cycle.StartDate.ToString("MMM dd, yyyy"));
                                table.Cell().Element(CellStyle).Text(cycle.EndDate.ToString("MMM dd, yyyy"));
                                table.Cell().Element(CellStyle).Text($"{cycle.InterestRate:N2}%");
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{cycle.TotalExpectedProfit:N2}");
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{paid:N2}");
                                table.Cell().Element(CellStyle).Text(cycle.Status.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                                }
                            }
                        });

                        // Totals
                        column.Item().PaddingTop(10);
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(text =>
                                {
                                    text.Span("Total Principal: ").Bold();
                                    text.Span($"{currencySymbol}{totalPrincipal:N2}");
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Total Expected Profit: ").Bold();
                                    text.Span($"{currencySymbol}{totalProfit:N2}");
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Total Paid: ").Bold();
                                    text.Span($"{currencySymbol}{totalPaid:N2}");
                                });
                            });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    #endregion

    #region Investment Summary

    public async Task<byte[]> GenerateInvestmentSummaryExcelAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var cycles = await _context.InvestmentCycles
            .Include(c => c.Participant)
            .Include(c => c.Payments)
            .ToListAsync();

        if (startDate.HasValue)
            cycles = cycles.Where(c => c.StartDate >= startDate.Value).ToList();
        if (endDate.HasValue)
            cycles = cycles.Where(c => c.StartDate <= endDate.Value).ToList();

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Investment Summary");

        // Header
        worksheet.Cell("A1").Value = companyName;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Font.Bold = true;

        worksheet.Cell("A2").Value = "Investment Summary Report";
        worksheet.Cell("A2").Style.Font.FontSize = 14;
        worksheet.Cell("A2").Style.Font.Bold = true;

        worksheet.Cell("A3").Value = $"Report Date: {DateTime.Now:MMMM dd, yyyy}";
        if (startDate.HasValue || endDate.HasValue)
        {
            var dateRange = $"Period: {startDate?.ToString("MMM dd, yyyy") ?? "Start"} - {endDate?.ToString("MMM dd, yyyy") ?? "Present"}";
            worksheet.Cell("A4").Value = dateRange;
        }

        int row = 6;

        // Summary Statistics
        worksheet.Cell(row, 1).Value = "Summary Statistics";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        var totalCycles = cycles.Count;
        var activeCycles = cycles.Count(c => c.Status == InvestmentStatus.Active);
        var completedCycles = cycles.Count(c => c.Status == InvestmentStatus.Completed);
        var totalPrincipal = cycles.Sum(c => c.PrincipalAmount);
        var totalProfit = cycles.Sum(c => c.TotalExpectedProfit);
        var totalPaid = cycles.Sum(c => c.Payments.Sum(p => p.Amount));

        worksheet.Cell(row, 1).Value = "Total Investment Cycles:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalCycles;
        row++;

        worksheet.Cell(row, 1).Value = "Active Cycles:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = activeCycles;
        row++;

        worksheet.Cell(row, 1).Value = "Completed Cycles:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = completedCycles;
        row++;

        worksheet.Cell(row, 1).Value = "Total Principal Amount:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalPrincipal;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row++;

        worksheet.Cell(row, 1).Value = "Total Expected Profit:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalProfit;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row++;

        worksheet.Cell(row, 1).Value = "Total Payments Received:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalPaid;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row += 2;

        // Detailed Table
        worksheet.Cell(row, 1).Value = "Investment Cycles Details";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        // Table Headers
        worksheet.Cell(row, 1).Value = "ID";
        worksheet.Cell(row, 2).Value = "Participant";
        worksheet.Cell(row, 3).Value = "Principal";
        worksheet.Cell(row, 4).Value = "Start Date";
        worksheet.Cell(row, 5).Value = "Duration";
        worksheet.Cell(row, 6).Value = "Interest Rate";
        worksheet.Cell(row, 7).Value = "Expected Profit";
        worksheet.Cell(row, 8).Value = "Total Paid";
        worksheet.Cell(row, 9).Value = "Status";

        var headerRange = worksheet.Range(row, 1, row, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        row++;

        foreach (var cycle in cycles.OrderBy(c => c.StartDate))
        {
            var paid = cycle.Payments.Sum(p => p.Amount);
            worksheet.Cell(row, 1).Value = cycle.Id;
            worksheet.Cell(row, 2).Value = cycle.Participant.FullName;
            worksheet.Cell(row, 3).Value = cycle.PrincipalAmount;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 4).Value = cycle.StartDate.ToString("MMM dd, yyyy");
            worksheet.Cell(row, 5).Value = $"{cycle.DurationMonths} months";
            worksheet.Cell(row, 6).Value = cycle.InterestRate;
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "0.00\"%\"";
            worksheet.Cell(row, 7).Value = cycle.TotalExpectedProfit;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 8).Value = paid;
            worksheet.Cell(row, 8).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 9).Value = cycle.Status.ToString();
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateInvestmentSummaryPdfAsync(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var cycles = await _context.InvestmentCycles
            .Include(c => c.Participant)
            .Include(c => c.Payments)
            .ToListAsync();

        if (startDate.HasValue)
            cycles = cycles.Where(c => c.StartDate >= startDate.Value).ToList();
        if (endDate.HasValue)
            cycles = cycles.Where(c => c.StartDate <= endDate.Value).ToList();

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        var totalCycles = cycles.Count;
        var activeCycles = cycles.Count(c => c.Status == InvestmentStatus.Active);
        var completedCycles = cycles.Count(c => c.Status == InvestmentStatus.Completed);
        var totalPrincipal = cycles.Sum(c => c.PrincipalAmount);
        var totalProfit = cycles.Sum(c => c.TotalExpectedProfit);
        var totalPaid = cycles.Sum(c => c.Payments.Sum(p => p.Amount));

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(companyName).FontSize(18).Bold();
                            column.Item().Text("Investment Summary Report").FontSize(14).Bold();
                            column.Item().PaddingTop(5).Text($"Report Date: {DateTime.Now:MMMM dd, yyyy}").FontSize(9);
                            if (startDate.HasValue || endDate.HasValue)
                            {
                                var dateRange = $"Period: {startDate?.ToString("MMM dd, yyyy") ?? "Start"} - {endDate?.ToString("MMM dd, yyyy") ?? "Present"}";
                                column.Item().Text(dateRange).FontSize(9);
                            }
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Summary Statistics
                        column.Item().Text("Summary Statistics").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Total Investment Cycles: {totalCycles}");
                                col.Item().Text($"Active Cycles: {activeCycles}");
                                col.Item().Text($"Completed Cycles: {completedCycles}");
                            });
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Total Principal: {currencySymbol}{totalPrincipal:N2}");
                                col.Item().Text($"Total Expected Profit: {currencySymbol}{totalProfit:N2}");
                                col.Item().Text($"Total Payments Received: {currencySymbol}{totalPaid:N2}");
                            });
                        });

                        column.Item().PaddingTop(15);

                        // Cycles Table
                        column.Item().Text("Investment Cycles Details").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);   // ID
                                columns.RelativeColumn(2);    // Participant
                                columns.RelativeColumn(1.5f); // Principal
                                columns.RelativeColumn(1.2f); // Start Date
                                columns.RelativeColumn(1);    // Duration
                                columns.RelativeColumn(0.8f); // Rate
                                columns.RelativeColumn(1.5f); // Profit
                                columns.RelativeColumn(1.5f); // Paid
                                columns.RelativeColumn(1);    // Status
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID").Bold();
                                header.Cell().Element(CellStyle).Text("Participant").Bold();
                                header.Cell().Element(CellStyle).Text("Principal").Bold();
                                header.Cell().Element(CellStyle).Text("Start Date").Bold();
                                header.Cell().Element(CellStyle).Text("Duration").Bold();
                                header.Cell().Element(CellStyle).Text("Rate").Bold();
                                header.Cell().Element(CellStyle).Text("Profit").Bold();
                                header.Cell().Element(CellStyle).Text("Paid").Bold();
                                header.Cell().Element(CellStyle).Text("Status").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten2).Padding(3);
                                }
                            });

                            // Data
                            foreach (var cycle in cycles.OrderBy(c => c.StartDate))
                            {
                                var paid = cycle.Payments.Sum(p => p.Amount);
                                table.Cell().Element(CellStyle).Text(cycle.Id.ToString());
                                table.Cell().Element(CellStyle).Text(cycle.Participant.FullName);
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{cycle.PrincipalAmount:N2}");
                                table.Cell().Element(CellStyle).Text(cycle.StartDate.ToString("MMM dd, yy"));
                                table.Cell().Element(CellStyle).Text($"{cycle.DurationMonths}m");
                                table.Cell().Element(CellStyle).Text($"{cycle.InterestRate:N2}%");
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{cycle.TotalExpectedProfit:N2}");
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{paid:N2}");
                                table.Cell().Element(CellStyle).Text(cycle.Status.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(3);
                                }
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    #endregion

    #region Monthly Cash Flow

    public async Task<byte[]> GenerateMonthlyCashFlowExcelAsync(int year, int month)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var startDate = DateOnly.FromDateTime(monthStart);
        var endDate = DateOnly.FromDateTime(monthEnd);

        var payments = await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(c => c.Participant)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Monthly Cash Flow");

        // Header
        worksheet.Cell("A1").Value = companyName;
        worksheet.Cell("A1").Style.Font.FontSize = 16;
        worksheet.Cell("A1").Style.Font.Bold = true;

        worksheet.Cell("A2").Value = "Monthly Cash Flow Report";
        worksheet.Cell("A2").Style.Font.FontSize = 14;
        worksheet.Cell("A2").Style.Font.Bold = true;

        worksheet.Cell("A3").Value = $"Period: {monthStart:MMMM yyyy}";
        worksheet.Cell("A4").Value = $"Report Date: {DateTime.Now:MMMM dd, yyyy}";

        int row = 6;

        // Summary Statistics
        worksheet.Cell(row, 1).Value = "Summary";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        var totalPayments = payments.Sum(p => p.Amount);
        var interestPayments = payments.Where(p => p.PaymentType == PaymentType.Interest).Sum(p => p.Amount);
        var principalPayments = payments.Where(p => p.PaymentType == PaymentType.Principal || p.PaymentType == PaymentType.PrincipalAndInterest).Sum(p => p.Amount);

        worksheet.Cell(row, 1).Value = "Total Payments Received:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = totalPayments;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row++;

        worksheet.Cell(row, 1).Value = "Interest Payments:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = interestPayments;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row++;

        worksheet.Cell(row, 1).Value = "Principal Payments:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = principalPayments;
        worksheet.Cell(row, 2).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
        row++;

        worksheet.Cell(row, 1).Value = "Number of Transactions:";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 2).Value = payments.Count;
        row += 2;

        // Payment Details
        worksheet.Cell(row, 1).Value = "Payment Details";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        // Table Headers
        worksheet.Cell(row, 1).Value = "Date";
        worksheet.Cell(row, 2).Value = "Participant";
        worksheet.Cell(row, 3).Value = "Amount";
        worksheet.Cell(row, 4).Value = "Payment Type";
        worksheet.Cell(row, 5).Value = "Payment Method";
        worksheet.Cell(row, 6).Value = "Reference";
        worksheet.Cell(row, 7).Value = "Received By";

        var headerRange = worksheet.Range(row, 1, row, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        row++;

        foreach (var payment in payments)
        {
            worksheet.Cell(row, 1).Value = payment.PaymentDate.ToString("MMM dd, yyyy");
            worksheet.Cell(row, 2).Value = payment.InvestmentCycle.Participant.FullName;
            worksheet.Cell(row, 3).Value = payment.Amount;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            worksheet.Cell(row, 4).Value = payment.PaymentType.ToString();
            worksheet.Cell(row, 5).Value = payment.PaymentMethod.ToString();
            worksheet.Cell(row, 6).Value = payment.ReferenceNumber ?? "";
            worksheet.Cell(row, 7).Value = payment.ReceivedBy ?? "";
            row++;
        }

        // Payment Method Breakdown
        row += 2;
        worksheet.Cell(row, 1).Value = "Payment Method Breakdown";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 12;
        row++;

        var methodGroups = payments.GroupBy(p => p.PaymentMethod)
            .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(p => p.Amount) })
            .OrderByDescending(x => x.Total);

        worksheet.Cell(row, 1).Value = "Method";
        worksheet.Cell(row, 2).Value = "Count";
        worksheet.Cell(row, 3).Value = "Total Amount";
        worksheet.Range(row, 1, row, 3).Style.Font.Bold = true;
        worksheet.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.LightGray;
        row++;

        foreach (var group in methodGroups)
        {
            worksheet.Cell(row, 1).Value = group.Method.ToString();
            worksheet.Cell(row, 2).Value = group.Count;
            worksheet.Cell(row, 3).Value = group.Total;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = $"\"{currencySymbol}\"#,##0.00";
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateMonthlyCashFlowPdfAsync(int year, int month)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var startDate = DateOnly.FromDateTime(monthStart);
        var endDate = DateOnly.FromDateTime(monthEnd);

        var payments = await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(c => c.Participant)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderBy(p => p.PaymentDate)
            .ToListAsync();

        var currencySymbol = await _settingService.GetValueAsync<string>("CurrencySymbol") ?? "$";
        var companyName = await _settingService.GetValueAsync<string>("CompanyName") ?? "Investment Tracker";

        var totalPayments = payments.Sum(p => p.Amount);
        var interestPayments = payments.Where(p => p.PaymentType == PaymentType.Interest).Sum(p => p.Amount);
        var principalPayments = payments.Where(p => p.PaymentType == PaymentType.Principal || p.PaymentType == PaymentType.PrincipalAndInterest).Sum(p => p.Amount);

        var methodGroups = payments.GroupBy(p => p.PaymentMethod)
            .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(p => p.Amount) })
            .OrderByDescending(x => x.Total)
            .ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text(companyName).FontSize(18).Bold();
                            column.Item().Text("Monthly Cash Flow Report").FontSize(14).Bold();
                            column.Item().PaddingTop(5).Text($"Period: {monthStart:MMMM yyyy}").FontSize(10).Bold();
                            column.Item().Text($"Report Date: {DateTime.Now:MMMM dd, yyyy}").FontSize(9);
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Summary Statistics
                        column.Item().Text("Summary").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Total Payments: {currencySymbol}{totalPayments:N2}").Bold();
                                col.Item().Text($"Interest Payments: {currencySymbol}{interestPayments:N2}");
                                col.Item().Text($"Principal Payments: {currencySymbol}{principalPayments:N2}");
                                col.Item().Text($"Number of Transactions: {payments.Count}");
                            });
                        });

                        column.Item().PaddingTop(15);

                        // Payment Details Table
                        column.Item().Text("Payment Details").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);    // Date
                                columns.RelativeColumn(2);    // Participant
                                columns.RelativeColumn(1.2f); // Amount
                                columns.RelativeColumn(1.2f); // Type
                                columns.RelativeColumn(1.2f); // Method
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date").Bold();
                                header.Cell().Element(CellStyle).Text("Participant").Bold();
                                header.Cell().Element(CellStyle).Text("Amount").Bold();
                                header.Cell().Element(CellStyle).Text("Type").Bold();
                                header.Cell().Element(CellStyle).Text("Method").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten2).Padding(5);
                                }
                            });

                            // Data
                            foreach (var payment in payments)
                            {
                                table.Cell().Element(CellStyle).Text(payment.PaymentDate.ToString("MMM dd"));
                                table.Cell().Element(CellStyle).Text(payment.InvestmentCycle.Participant.FullName);
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{payment.Amount:N2}");
                                table.Cell().Element(CellStyle).Text(payment.PaymentType.ToString());
                                table.Cell().Element(CellStyle).Text(payment.PaymentMethod.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                                }
                            }
                        });

                        // Payment Method Breakdown
                        column.Item().PaddingTop(15);
                        column.Item().Text("Payment Method Breakdown").FontSize(12).Bold();
                        column.Item().PaddingTop(5);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Method").Bold();
                                header.Cell().Element(CellStyle).Text("Count").Bold();
                                header.Cell().Element(CellStyle).Text("Total Amount").Bold();

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.Background(Colors.Grey.Lighten2).Padding(5);
                                }
                            });

                            foreach (var group in methodGroups)
                            {
                                table.Cell().Element(CellStyle).Text(group.Method.ToString());
                                table.Cell().Element(CellStyle).Text(group.Count.ToString());
                                table.Cell().Element(CellStyle).Text($"{currencySymbol}{group.Total:N2}");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
                                }
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    #endregion
}
