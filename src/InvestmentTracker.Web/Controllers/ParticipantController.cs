using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Web.Controllers;

/// <summary>
/// Controller for managing participants
/// </summary>
public class ParticipantController : Controller
{
    private readonly IParticipantService _participantService;
    private readonly IInvestmentCycleService _investmentCycleService;

    public ParticipantController(
        IParticipantService participantService,
        IInvestmentCycleService investmentCycleService)
    {
        _participantService = participantService;
        _investmentCycleService = investmentCycleService;
    }

    // GET: Participant
    public async Task<IActionResult> Index()
    {
        var participants = await _participantService.GetAllAsync();
        return View(participants);
    }

    // GET: Participant/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            return NotFound();
        }

        // Get participant's investment cycles
        ViewBag.InvestmentCycles = await _investmentCycleService.GetByParticipantIdAsync(id);

        return View(participant);
    }

    // GET: Participant/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Participant/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateParticipantDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        // Validate email uniqueness
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (await _participantService.EmailExistsAsync(dto.Email))
            {
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");
                return View(dto);
            }
        }

        // Validate national ID uniqueness
        if (!string.IsNullOrWhiteSpace(dto.NationalId))
        {
            if (await _participantService.NationalIdExistsAsync(dto.NationalId))
            {
                ModelState.AddModelError(nameof(dto.NationalId), "National ID already exists.");
                return View(dto);
            }
        }

        try
        {
            await _participantService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Participant created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error creating participant: {ex.Message}");
            return View(dto);
        }
    }

    // GET: Participant/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            return NotFound();
        }

        var dto = new CreateParticipantDto
        {
            FullName = participant.FullName,
            Email = participant.Email,
            Phone = participant.Phone,
            NationalId = participant.NationalId,
            Address = participant.Address,
            DateOfBirth = participant.DateOfBirth,
            RegistrationDate = participant.RegistrationDate,
            Notes = participant.Notes
        };

        return View(dto);
    }

    // POST: Participant/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateParticipantDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        // Validate email uniqueness (excluding current record)
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (await _participantService.EmailExistsAsync(dto.Email, id))
            {
                ModelState.AddModelError(nameof(dto.Email), "Email already exists.");
                return View(dto);
            }
        }

        // Validate national ID uniqueness (excluding current record)
        if (!string.IsNullOrWhiteSpace(dto.NationalId))
        {
            if (await _participantService.NationalIdExistsAsync(dto.NationalId, id))
            {
                ModelState.AddModelError(nameof(dto.NationalId), "National ID already exists.");
                return View(dto);
            }
        }

        try
        {
            await _participantService.UpdateAsync(id, dto);
            TempData["SuccessMessage"] = "Participant updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error updating participant: {ex.Message}");
            return View(dto);
        }
    }

    // GET: Participant/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            return NotFound();
        }

        return View(participant);
    }

    // POST: Participant/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var result = await _participantService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Participant deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting participant: {ex.Message}";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    // GET: Participant/Search
    public async Task<IActionResult> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return RedirectToAction(nameof(Index));
        }

        var participants = await _participantService.SearchAsync(term);
        ViewBag.SearchTerm = term;
        return View("Index", participants);
    }
}
