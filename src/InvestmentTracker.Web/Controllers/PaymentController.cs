using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Controller for managing payments
/// </summary>
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IInvestmentCycleService _investmentCycleService;
    private readonly IPaymentScheduleService _paymentScheduleService;

    public PaymentController(
        IPaymentService paymentService,
        IInvestmentCycleService investmentCycleService,
        IPaymentScheduleService paymentScheduleService)
    {
        _paymentService = paymentService;
        _investmentCycleService = investmentCycleService;
        _paymentScheduleService = paymentScheduleService;
    }

    // GET: Payment
    public async Task<IActionResult> Index(DateOnly? startDate = null, DateOnly? endDate = null)
    {
        IEnumerable<PaymentDto> payments;

        if (startDate.HasValue && endDate.HasValue)
        {
            payments = await _paymentService.GetByDateRangeAsync(startDate.Value, endDate.Value);
            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;
        }
        else
        {
            payments = await _paymentService.GetAllAsync();
        }

        return View(payments);
    }

    // GET: Payment/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // GET: Payment/Create
    public async Task<IActionResult> Create(int? investmentCycleId = null, int? scheduleId = null)
    {
        await LoadInvestmentCyclesSelectList();
        LoadPaymentTypesAndMethods();

        var dto = new CreatePaymentDto
        {
            PaymentDate = DateOnly.FromDateTime(DateTime.Today),
            PaymentType = PaymentType.Interest,
            PaymentMethod = PaymentMethod.Cash
        };

        if (investmentCycleId.HasValue)
        {
            dto.InvestmentCycleId = investmentCycleId.Value;
            await LoadPaymentSchedulesForCycle(investmentCycleId.Value);
        }

        if (scheduleId.HasValue)
        {
            dto.PaymentScheduleId = scheduleId.Value;
            
            // Pre-fill amount from schedule
            var schedule = await _paymentScheduleService.GetByIdAsync(scheduleId.Value);
            if (schedule != null)
            {
                dto.Amount = schedule.RemainingAmount;
                dto.PaymentType = schedule.PaymentType;
            }
        }

        return View(dto);
    }

    // POST: Payment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePaymentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }

        // Validate amount
        if (dto.Amount <= 0)
        {
            ModelState.AddModelError(nameof(dto.Amount), "Payment amount must be greater than zero.");
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }

        try
        {
            var payment = await _paymentService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Payment recorded successfully.";
            return RedirectToAction("Details", "InvestmentCycle", new { id = dto.InvestmentCycleId });
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error recording payment: {ex.Message}");
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }
    }

    // GET: Payment/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        await LoadInvestmentCyclesSelectList();
        LoadPaymentTypesAndMethods();
        await LoadPaymentSchedulesForCycle(payment.InvestmentCycleId);

        var dto = new CreatePaymentDto
        {
            InvestmentCycleId = payment.InvestmentCycleId,
            PaymentScheduleId = payment.PaymentScheduleId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentType = payment.PaymentType,
            PaymentMethod = payment.PaymentMethod,
            ReferenceNumber = payment.ReferenceNumber,
            ReceivedBy = payment.ReceivedBy,
            Notes = payment.Notes
        };

        return View(dto);
    }

    // POST: Payment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreatePaymentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }

        if (dto.Amount <= 0)
        {
            ModelState.AddModelError(nameof(dto.Amount), "Payment amount must be greater than zero.");
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }

        try
        {
            await _paymentService.UpdateAsync(id, dto);
            TempData["SuccessMessage"] = "Payment updated successfully.";
            return RedirectToAction("Details", "InvestmentCycle", new { id = dto.InvestmentCycleId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error updating payment: {ex.Message}");
            await LoadInvestmentCyclesSelectList();
            LoadPaymentTypesAndMethods();
            if (dto.InvestmentCycleId > 0)
            {
                await LoadPaymentSchedulesForCycle(dto.InvestmentCycleId);
            }
            return View(dto);
        }
    }

    // GET: Payment/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

    // POST: Payment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
        {
            return NotFound();
        }

        var investmentCycleId = payment.InvestmentCycleId;

        try
        {
            var result = await _paymentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Payment deleted successfully.";
            return RedirectToAction("Details", "InvestmentCycle", new { id = investmentCycleId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting payment: {ex.Message}";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // GET: Payment/Recent
    public async Task<IActionResult> Recent(int days = 30)
    {
        var payments = await _paymentService.GetRecentAsync(days);
        ViewBag.Days = days;
        return View("Index", payments);
    }

    private async Task LoadInvestmentCyclesSelectList()
    {
        var cycles = await _investmentCycleService.GetByStatusAsync(InvestmentStatus.Active);
        ViewBag.InvestmentCycles = new SelectList(
            cycles.Select(c => new
            {
                c.Id,
                DisplayName = $"{c.ParticipantName} - {c.StartDate:dd/MM/yyyy} ({c.PrincipalAmount:N0})"
            }),
            "Id",
            "DisplayName"
        );
    }

    private async Task LoadPaymentSchedulesForCycle(int cycleId)
    {
        var schedules = await _paymentScheduleService.GetByInvestmentCycleIdAsync(cycleId);
        ViewBag.PaymentSchedules = new SelectList(
            schedules
                .Where(s => s.Status != PaymentStatus.Paid)
                .Select(s => new
                {
                    s.Id,
                    DisplayName = $"{s.ScheduledDate:dd/MM/yyyy} - {s.ScheduledAmount:N2} ({s.PaymentType}) - Remaining: {s.RemainingAmount:N2}"
                }),
            "Id",
            "DisplayName"
        );
    }

    private void LoadPaymentTypesAndMethods()
    {
        ViewBag.PaymentTypes = new SelectList(Enum.GetValues(typeof(PaymentType)));
        ViewBag.PaymentMethods = new SelectList(Enum.GetValues(typeof(PaymentMethod)));
    }
}
