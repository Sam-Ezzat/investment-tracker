using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Enums;
using InvestmentTracker.Web.Models.ViewModels;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Dashboard controller for displaying statistics and overview
/// </summary>
public class DashboardController : Controller
{
    private readonly IParticipantService _participantService;
    private readonly IInvestmentCycleService _investmentCycleService;
    private readonly IPaymentService _paymentService;
    private readonly IPaymentScheduleService _paymentScheduleService;

    public DashboardController(
        IParticipantService participantService,
        IInvestmentCycleService investmentCycleService,
        IPaymentService paymentService,
        IPaymentScheduleService paymentScheduleService)
    {
        _participantService = participantService;
        _investmentCycleService = investmentCycleService;
        _paymentService = paymentService;
        _paymentScheduleService = paymentScheduleService;
    }

    // GET: Dashboard
    public async Task<IActionResult> Index()
    {
        try
        {
            var viewModel = new DashboardViewModel();

            // Get all data
            var allParticipants = await _participantService.GetAllAsync();
            var allCycles = await _investmentCycleService.GetAllAsync();
            var allPayments = await _paymentService.GetAllAsync();
            var recentPayments = await _paymentService.GetRecentAsync(30);
            var overdueSchedules = await _paymentScheduleService.GetOverdueAsync();
            var dueSoonSchedules = await _paymentScheduleService.GetDueSoonAsync(7);
            var activeCycles = await _investmentCycleService.GetByStatusAsync(InvestmentStatus.Active);
            var endingSoon = await _investmentCycleService.GetEndingSoonAsync(30);
            
            // Get today's due schedules
            var today = DateOnly.FromDateTime(DateTime.Today);
            var allSchedules = activeCycles.SelectMany(c => 
                _paymentScheduleService.GetByInvestmentCycleIdAsync(c.Id).Result);
            var todaysDue = allSchedules.Where(s => 
                s.ScheduledDate == today && 
                (s.Status == PaymentStatus.Pending || s.Status == PaymentStatus.Due)).ToList();

            // Calculate statistics
            viewModel.TotalParticipants = allParticipants.Count();
            viewModel.ActiveParticipants = allParticipants.Count(p => p.IsActive);

            viewModel.TotalInvestmentCycles = allCycles.Count();
            viewModel.ActiveInvestmentCycles = activeCycles.Count();
            viewModel.CompletedInvestmentCycles = allCycles.Count(c => c.Status == InvestmentStatus.Completed);

            viewModel.TotalInvestedAmount = allCycles.Sum(c => c.PrincipalAmount);
            viewModel.TotalExpectedProfit = activeCycles.Sum(c => c.TotalExpectedProfit);
            viewModel.TotalExpectedReturn = activeCycles.Sum(c => c.FinalAmount);

            viewModel.TotalPaidAmount = allCycles.Sum(c => c.TotalPaidAmount);
            viewModel.TotalRemainingAmount = activeCycles.Sum(c => c.RemainingAmount);

            viewModel.RecentPaymentsCount = recentPayments.Count();
            viewModel.RecentPaymentsTotal = recentPayments.Sum(p => p.Amount);

            viewModel.OverduePaymentsCount = overdueSchedules.Count();
            viewModel.OverduePaymentsTotal = overdueSchedules.Sum(s => s.RemainingAmount);

            viewModel.DueSoonPaymentsCount = dueSoonSchedules.Count();
            viewModel.DueSoonPaymentsTotal = dueSoonSchedules.Sum(s => s.RemainingAmount);

            viewModel.TodaysDueCount = todaysDue.Count();
            viewModel.TodaysDueTotal = todaysDue.Sum(s => s.RemainingAmount);

            viewModel.CyclesEndingSoonCount = endingSoon.Count();

            // Calculate monthly profit (current month)
            var currentMonth = DateTime.Today;
            var monthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            var monthPayments = allPayments.Where(p => 
                p.PaymentDate >= DateOnly.FromDateTime(monthStart) && 
                p.PaymentDate <= DateOnly.FromDateTime(monthEnd) &&
                p.PaymentType == PaymentType.Interest);
            viewModel.MonthlyProfit = monthPayments.Sum(p => p.Amount);

            // Recent payments
            viewModel.RecentPayments = recentPayments.Take(10);

            // Overdue schedules
            viewModel.OverdueSchedules = overdueSchedules.Take(10);

            // Due soon schedules
            viewModel.DueSoonSchedules = dueSoonSchedules.Take(10);

            // Today's due schedules
            viewModel.TodaysDueSchedules = todaysDue.Take(10);

            // Cycles ending soon
            viewModel.CyclesEndingSoon = endingSoon.Take(10);

            // Top participants by invested amount
            viewModel.TopParticipants = allParticipants
                .OrderByDescending(p => p.TotalInvestedAmount)
                .Take(5);

            // Chart Data - Monthly Profit (last 6 months)
            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var monthStartDate = new DateTime(month.Year, month.Month, 1);
                var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);
                var monthName = month.ToString("MMM yyyy");
                
                var monthProfit = allPayments.Where(p => 
                    p.PaymentDate >= DateOnly.FromDateTime(monthStartDate) && 
                    p.PaymentDate <= DateOnly.FromDateTime(monthEndDate) &&
                    p.PaymentType == PaymentType.Interest)
                    .Sum(p => p.Amount);
                
                viewModel.MonthlyProfitData[monthName] = monthProfit;
            }

            // Chart Data - Cycles by Status
            viewModel.CyclesByStatusData["Active"] = allCycles.Count(c => c.Status == InvestmentStatus.Active);
            viewModel.CyclesByStatusData["Completed"] = allCycles.Count(c => c.Status == InvestmentStatus.Completed);
            viewModel.CyclesByStatusData["Cancelled"] = allCycles.Count(c => c.Status == InvestmentStatus.Cancelled);
            viewModel.CyclesByStatusData["On Hold"] = allCycles.Count(c => c.Status == InvestmentStatus.OnHold);

            // Chart Data - Payment Methods
            viewModel.PaymentMethodsData["Cash"] = allPayments.Count(p => p.PaymentMethod == PaymentMethod.Cash);
            viewModel.PaymentMethodsData["Bank Transfer"] = allPayments.Count(p => p.PaymentMethod == PaymentMethod.BankTransfer);
            viewModel.PaymentMethodsData["Cheque"] = allPayments.Count(p => p.PaymentMethod == PaymentMethod.Cheque);
            viewModel.PaymentMethodsData["Online"] = allPayments.Count(p => p.PaymentMethod == PaymentMethod.Online);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error loading dashboard: {ex.Message}";
            return View(new DashboardViewModel());
        }
    }

    // POST: Dashboard/RefreshOverdueStatus
    [HttpPost]
    public async Task<IActionResult> RefreshOverdueStatus()
    {
        try
        {
            await _paymentScheduleService.UpdateOverdueStatusesAsync();
            TempData["SuccessMessage"] = "Overdue statuses updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating overdue statuses: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
