using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Enums;
using InvestmentTracker.Infrastructure.Data;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for Investment Cycle operations with business logic
/// </summary>
public class InvestmentCycleService : IInvestmentCycleService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentScheduleService _paymentScheduleService;

    public InvestmentCycleService(
        ApplicationDbContext context,
        IPaymentScheduleService paymentScheduleService)
    {
        _context = context;
        _paymentScheduleService = paymentScheduleService;
    }

    public async Task<IEnumerable<InvestmentCycleDto>> GetAllAsync()
    {
        return await _context.InvestmentCycles
            .Include(ic => ic.Participant)
            .Include(ic => ic.Payments)
            .OrderByDescending(ic => ic.StartDate)
            .Select(ic => MapToDto(ic))
            .ToListAsync();
    }

    public async Task<IEnumerable<InvestmentCycleDto>> GetByParticipantIdAsync(int participantId)
    {
        return await _context.InvestmentCycles
            .Include(ic => ic.Participant)
            .Include(ic => ic.Payments)
            .Where(ic => ic.ParticipantId == participantId)
            .OrderByDescending(ic => ic.StartDate)
            .Select(ic => MapToDto(ic))
            .ToListAsync();
    }

    public async Task<IEnumerable<InvestmentCycleDto>> GetByStatusAsync(InvestmentStatus status)
    {
        return await _context.InvestmentCycles
            .Include(ic => ic.Participant)
            .Include(ic => ic.Payments)
            .Where(ic => ic.Status == status)
            .OrderByDescending(ic => ic.StartDate)
            .Select(ic => MapToDto(ic))
            .ToListAsync();
    }

    public async Task<InvestmentCycleDto?> GetByIdAsync(int id)
    {
        var cycle = await _context.InvestmentCycles
            .Include(ic => ic.Participant)
            .Include(ic => ic.Payments)
            .FirstOrDefaultAsync(ic => ic.Id == id);

        return cycle != null ? MapToDto(cycle) : null;
    }

    public async Task<InvestmentCycleDto> CreateAsync(CreateInvestmentCycleDto dto)
    {
        // Validate participant exists
        var participantExists = await _context.Participants.AnyAsync(p => p.Id == dto.ParticipantId);
        if (!participantExists)
            throw new KeyNotFoundException($"Participant with ID {dto.ParticipantId} not found.");

        // Calculate all investment values
        var calculations = CalculateInvestmentValues(dto);

        // Create investment cycle entity
        var cycle = new InvestmentCycle
        {
            ParticipantId = dto.ParticipantId,
            PrincipalAmount = dto.PrincipalAmount,
            StartDate = dto.StartDate,
            DurationMonths = dto.DurationMonths,
            EndDate = calculations.EndDate,
            InterestType = dto.InterestType,
            InterestRate = dto.InterestRate,
            MonthlyInterest = calculations.MonthlyInterest,
            TotalExpectedProfit = calculations.TotalExpectedProfit,
            FinalAmount = calculations.FinalAmount,
            Status = InvestmentStatus.Active,
            Notes = dto.Notes
        };

        _context.InvestmentCycles.Add(cycle);
        await _context.SaveChangesAsync();

        // Generate payment schedules automatically
        await _paymentScheduleService.GenerateSchedulesAsync(cycle.Id);

        return (await GetByIdAsync(cycle.Id))!;
    }

    public async Task<InvestmentCycleDto> UpdateAsync(int id, CreateInvestmentCycleDto dto)
    {
        var cycle = await _context.InvestmentCycles.FindAsync(id);
        if (cycle == null)
            throw new KeyNotFoundException($"Investment cycle with ID {id} not found.");

        // Recalculate values
        var calculations = CalculateInvestmentValues(dto);

        cycle.ParticipantId = dto.ParticipantId;
        cycle.PrincipalAmount = dto.PrincipalAmount;
        cycle.StartDate = dto.StartDate;
        cycle.DurationMonths = dto.DurationMonths;
        cycle.EndDate = calculations.EndDate;
        cycle.InterestType = dto.InterestType;
        cycle.InterestRate = dto.InterestRate;
        cycle.MonthlyInterest = calculations.MonthlyInterest;
        cycle.TotalExpectedProfit = calculations.TotalExpectedProfit;
        cycle.FinalAmount = calculations.FinalAmount;
        cycle.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        // Regenerate payment schedules
        // Delete existing schedules
        var existingSchedules = await _context.PaymentSchedules
            .Where(ps => ps.InvestmentCycleId == id)
            .ToListAsync();
        _context.PaymentSchedules.RemoveRange(existingSchedules);
        await _context.SaveChangesAsync();

        // Generate new schedules
        await _paymentScheduleService.GenerateSchedulesAsync(id);

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> UpdateStatusAsync(int id, InvestmentStatus status)
    {
        var cycle = await _context.InvestmentCycles.FindAsync(id);
        if (cycle == null)
            return false;

        cycle.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cycle = await _context.InvestmentCycles.FindAsync(id);
        if (cycle == null)
            return false;

        // Check if there are any payments
        var hasPayments = await _context.Payments.AnyAsync(p => p.InvestmentCycleId == id);
        if (hasPayments)
            throw new InvalidOperationException("Cannot delete investment cycle with existing payments.");

        _context.InvestmentCycles.Remove(cycle);
        await _context.SaveChangesAsync();

        return true;
    }

    public Task<InvestmentCycleDto> CalculateInvestmentAsync(CreateInvestmentCycleDto dto)
    {
        var calculations = CalculateInvestmentValues(dto);

        var result = new InvestmentCycleDto
        {
            ParticipantId = dto.ParticipantId,
            PrincipalAmount = dto.PrincipalAmount,
            StartDate = dto.StartDate,
            DurationMonths = dto.DurationMonths,
            EndDate = calculations.EndDate,
            InterestType = dto.InterestType,
            InterestRate = dto.InterestRate,
            MonthlyInterest = calculations.MonthlyInterest,
            TotalExpectedProfit = calculations.TotalExpectedProfit,
            FinalAmount = calculations.FinalAmount,
            Notes = dto.Notes
        };

        return Task.FromResult(result);
    }

    public async Task<IEnumerable<InvestmentCycleDto>> GetEndingSoonAsync(int daysAhead = 30)
    {
        var targetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(daysAhead));
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _context.InvestmentCycles
            .Include(ic => ic.Participant)
            .Include(ic => ic.Payments)
            .Where(ic => ic.Status == InvestmentStatus.Active &&
                        ic.EndDate >= today &&
                        ic.EndDate <= targetDate)
            .OrderBy(ic => ic.EndDate)
            .Select(ic => MapToDto(ic))
            .ToListAsync();
    }

    #region Private Helper Methods

    /// <summary>
    /// Calculate all investment values based on input
    /// </summary>
    private (DateOnly EndDate, decimal MonthlyInterest, decimal TotalExpectedProfit, decimal FinalAmount) 
        CalculateInvestmentValues(CreateInvestmentCycleDto dto)
    {
        // Calculate End Date
        var endDate = dto.StartDate.AddMonths(dto.DurationMonths);

        decimal monthlyInterest = 0;
        decimal totalExpectedProfit = 0;

        if (dto.InterestType == InterestType.MonthlyPercentage)
        {
            // Monthly percentage interest
            // MonthlyInterest = Principal * (Rate / 100)
            monthlyInterest = dto.PrincipalAmount * (dto.InterestRate / 100);
            
            // Total Profit = MonthlyInterest * Duration
            totalExpectedProfit = monthlyInterest * dto.DurationMonths;
        }
        else if (dto.InterestType == InterestType.FixedProfitAtEnd)
        {
            // Fixed profit at end
            // The InterestRate field contains the fixed profit amount
            totalExpectedProfit = dto.InterestRate;
            monthlyInterest = 0; // No monthly interest for fixed profit
        }

        // Final Amount = Principal + Total Profit
        var finalAmount = dto.PrincipalAmount + totalExpectedProfit;

        return (endDate, monthlyInterest, totalExpectedProfit, finalAmount);
    }

    /// <summary>
    /// Map entity to DTO with calculated fields
    /// </summary>
    private static InvestmentCycleDto MapToDto(InvestmentCycle ic)
    {
        var totalPaid = ic.Payments.Sum(p => p.Amount);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var daysRemaining = ic.EndDate.DayNumber - today.DayNumber;

        return new InvestmentCycleDto
        {
            Id = ic.Id,
            ParticipantId = ic.ParticipantId,
            ParticipantName = ic.Participant.FullName,
            PrincipalAmount = ic.PrincipalAmount,
            StartDate = ic.StartDate,
            DurationMonths = ic.DurationMonths,
            EndDate = ic.EndDate,
            InterestType = ic.InterestType,
            InterestRate = ic.InterestRate,
            MonthlyInterest = ic.MonthlyInterest,
            TotalExpectedProfit = ic.TotalExpectedProfit,
            FinalAmount = ic.FinalAmount,
            Status = ic.Status,
            Notes = ic.Notes,
            TotalPaidAmount = totalPaid,
            RemainingAmount = ic.FinalAmount - totalPaid,
            DaysRemaining = daysRemaining > 0 ? daysRemaining : 0
        };
    }

    #endregion
}
