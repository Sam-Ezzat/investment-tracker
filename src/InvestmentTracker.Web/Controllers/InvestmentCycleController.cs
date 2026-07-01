using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Controller for managing investment cycles
/// </summary>
public class InvestmentCycleController : Controller
{
    private readonly IInvestmentCycleService _investmentCycleService;
    private readonly IParticipantService _participantService;
    private readonly IPaymentScheduleService _paymentScheduleService;
    private readonly IPaymentService _paymentService;

    public InvestmentCycleController(
        IInvestmentCycleService investmentCycleService,
        IParticipantService participantService,
        IPaymentScheduleService paymentScheduleService,
        IPaymentService paymentService)
    {
        _investmentCycleService = investmentCycleService;
        _participantService = participantService;
        _paymentScheduleService = paymentScheduleService;
        _paymentService = paymentService;
    }

    // GET: InvestmentCycle
    public async Task<IActionResult> Index(InvestmentStatus? status = null)
    {
        IEnumerable<InvestmentCycleDto> cycles;

        if (status.HasValue)
        {
            cycles = await _investmentCycleService.GetByStatusAsync(status.Value);
            ViewBag.CurrentStatus = status.Value;
        }
        else
        {
            cycles = await _investmentCycleService.GetAllAsync();
        }

        return View(cycles);
    }

    // GET: InvestmentCycle/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var cycle = await _investmentCycleService.GetByIdAsync(id);
        if (cycle == null)
        {
            return NotFound();
        }

        // Get payment schedules and payments
        ViewBag.PaymentSchedules = await _paymentScheduleService.GetByInvestmentCycleIdAsync(id);
        ViewBag.Payments = await _paymentService.GetByInvestmentCycleIdAsync(id);

        return View(cycle);
    }

    // GET: InvestmentCycle/Create
    public async Task<IActionResult> Create(int? participantId = null)
    {
        await LoadParticipantsSelectList();
        
        var dto = new CreateInvestmentCycleDto
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            DurationMonths = 12,
            InterestType = InterestType.MonthlyPercentage,
            InterestRate = 5.0m
        };

        if (participantId.HasValue)
        {
            dto.ParticipantId = participantId.Value;
        }

        return View(dto);
    }

    // POST: InvestmentCycle/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInvestmentCycleDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadParticipantsSelectList();
            return View(dto);
        }

        // Validate principal amount
        if (dto.PrincipalAmount <= 0)
        {
            ModelState.AddModelError(nameof(dto.PrincipalAmount), "Principal amount must be greater than zero.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        // Validate duration
        if (dto.DurationMonths <= 0)
        {
            ModelState.AddModelError(nameof(dto.DurationMonths), "Duration must be greater than zero.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        // Validate interest rate
        if (dto.InterestRate < 0)
        {
            ModelState.AddModelError(nameof(dto.InterestRate), "Interest rate cannot be negative.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        try
        {
            var cycle = await _investmentCycleService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Investment cycle created successfully. Payment schedules have been generated.";
            return RedirectToAction(nameof(Details), new { id = cycle.Id });
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadParticipantsSelectList();
            return View(dto);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating investment cycle: {ex.Message}");
            await LoadParticipantsSelectList();
            return View(dto);
        }
    }

    // GET: InvestmentCycle/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var cycle = await _investmentCycleService.GetByIdAsync(id);
        if (cycle == null)
        {
            return NotFound();
        }

        await LoadParticipantsSelectList();

        var dto = new CreateInvestmentCycleDto
        {
            ParticipantId = cycle.ParticipantId,
            PrincipalAmount = cycle.PrincipalAmount,
            StartDate = cycle.StartDate,
            DurationMonths = cycle.DurationMonths,
            InterestType = cycle.InterestType,
            InterestRate = cycle.InterestRate,
            Notes = cycle.Notes
        };

        return View(dto);
    }

    // POST: InvestmentCycle/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateInvestmentCycleDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadParticipantsSelectList();
            return View(dto);
        }

        // Validate amounts
        if (dto.PrincipalAmount <= 0)
        {
            ModelState.AddModelError(nameof(dto.PrincipalAmount), "Principal amount must be greater than zero.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        if (dto.DurationMonths <= 0)
        {
            ModelState.AddModelError(nameof(dto.DurationMonths), "Duration must be greater than zero.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        if (dto.InterestRate < 0)
        {
            ModelState.AddModelError(nameof(dto.InterestRate), "Interest rate cannot be negative.");
            await LoadParticipantsSelectList();
            return View(dto);
        }

        try
        {
            await _investmentCycleService.UpdateAsync(id, dto);
            TempData["SuccessMessage"] = "Investment cycle updated successfully. Payment schedules have been regenerated.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error updating investment cycle: {ex.Message}");
            await LoadParticipantsSelectList();
            return View(dto);
        }
    }

    // POST: InvestmentCycle/UpdateStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, InvestmentStatus status)
    {
        try
        {
            var result = await _investmentCycleService.UpdateStatusAsync(id, status);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = $"Investment cycle status updated to {status}.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error updating status: {ex.Message}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // GET: InvestmentCycle/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var cycle = await _investmentCycleService.GetByIdAsync(id);
        if (cycle == null)
        {
            return NotFound();
        }

        return View(cycle);
    }

    // POST: InvestmentCycle/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _investmentCycleService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Investment cycle deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Delete), new { id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting investment cycle: {ex.Message}";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // GET: InvestmentCycle/Calculate
    [HttpPost]
    public async Task<IActionResult> Calculate([FromBody] CreateInvestmentCycleDto dto)
    {
        if (dto == null)
        {
            return BadRequest();
        }

        try
        {
            var calculation = await _investmentCycleService.CalculateInvestmentAsync(dto);
            return Json(new
            {
                endDate = calculation.EndDate.ToString("yyyy-MM-dd"),
                monthlyInterest = calculation.MonthlyInterest,
                totalExpectedProfit = calculation.TotalExpectedProfit,
                finalAmount = calculation.FinalAmount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private async Task LoadParticipantsSelectList()
    {
        var participants = await _participantService.GetActiveAsync();
        ViewBag.Participants = new SelectList(participants, "Id", "FullName");
    }
}
