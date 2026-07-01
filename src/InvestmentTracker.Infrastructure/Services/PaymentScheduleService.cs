using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Enums;
using InvestmentTracker.Infrastructure.Data;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for Payment Schedule operations
/// </summary>
public class PaymentScheduleService : IPaymentScheduleService
{
    private readonly ApplicationDbContext _context;

    public PaymentScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentScheduleDto>> GetByInvestmentCycleIdAsync(int investmentCycleId)
    {
        return await _context.PaymentSchedules
            .Where(ps => ps.InvestmentCycleId == investmentCycleId)
            .Select(ps => MapToDto(ps))
            .OrderBy(ps => ps.ScheduledDate)
            .ToListAsync();
    }

    public async Task<PaymentScheduleDto?> GetByIdAsync(int id)
    {
        var schedule = await _context.PaymentSchedules.FindAsync(id);
        return schedule != null ? MapToDto(schedule) : null;
    }

    public async Task<IEnumerable<PaymentScheduleDto>> GetByStatusAsync(PaymentStatus status)
    {
        return await _context.PaymentSchedules
            .Include(ps => ps.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(ps => ps.Status == status)
            .Select(ps => MapToDto(ps))
            .OrderBy(ps => ps.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentScheduleDto>> GetOverdueAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _context.PaymentSchedules
            .Include(ps => ps.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(ps => ps.ScheduledDate < today && 
                        ps.Status != PaymentStatus.Paid)
            .Select(ps => MapToDto(ps))
            .OrderBy(ps => ps.ScheduledDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentScheduleDto>> GetDueSoonAsync(int daysAhead = 7)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var targetDate = today.AddDays(daysAhead);

        return await _context.PaymentSchedules
            .Include(ps => ps.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(ps => ps.ScheduledDate >= today && 
                        ps.ScheduledDate <= targetDate &&
                        ps.Status != PaymentStatus.Paid)
            .Select(ps => MapToDto(ps))
            .OrderBy(ps => ps.ScheduledDate)
            .ToListAsync();
    }

    public async Task GenerateSchedulesAsync(int investmentCycleId)
    {
        var cycle = await _context.InvestmentCycles.FindAsync(investmentCycleId);
        if (cycle == null)
            throw new KeyNotFoundException($"Investment cycle with ID {investmentCycleId} not found.");

        var schedules = new List<PaymentSchedule>();

        if (cycle.InterestType == InterestType.MonthlyPercentage)
        {
            // Generate monthly interest payments
            for (int month = 1; month <= cycle.DurationMonths; month++)
            {
                var scheduleDate = cycle.StartDate.AddMonths(month);

                schedules.Add(new PaymentSchedule
                {
                    InvestmentCycleId = investmentCycleId,
                    ScheduledDate = scheduleDate,
                    ScheduledAmount = cycle.MonthlyInterest,
                    PaymentType = PaymentType.Interest,
                    Status = PaymentStatus.Pending,
                    PaidAmount = 0
                });
            }

            // Add final principal payment at the end
            schedules.Add(new PaymentSchedule
            {
                InvestmentCycleId = investmentCycleId,
                ScheduledDate = cycle.EndDate,
                ScheduledAmount = cycle.PrincipalAmount,
                PaymentType = PaymentType.Principal,
                Status = PaymentStatus.Pending,
                PaidAmount = 0
            });
        }
        else if (cycle.InterestType == InterestType.FixedProfitAtEnd)
        {
            // Single payment at the end (Principal + Profit)
            schedules.Add(new PaymentSchedule
            {
                InvestmentCycleId = investmentCycleId,
                ScheduledDate = cycle.EndDate,
                ScheduledAmount = cycle.FinalAmount,
                PaymentType = PaymentType.PrincipalAndInterest,
                Status = PaymentStatus.Pending,
                PaidAmount = 0
            });
        }

        _context.PaymentSchedules.AddRange(schedules);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateScheduleStatusAsync(int scheduleId, decimal paidAmount)
    {
        var schedule = await _context.PaymentSchedules.FindAsync(scheduleId);
        if (schedule == null)
            return;

        schedule.PaidAmount += paidAmount;

        // Update status based on payment
        if (schedule.PaidAmount >= schedule.ScheduledAmount)
        {
            schedule.Status = PaymentStatus.Paid;
            schedule.PaidDate = DateOnly.FromDateTime(DateTime.Today);
        }
        else if (schedule.PaidAmount > 0)
        {
            schedule.Status = PaymentStatus.PartiallyPaid;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateOverdueStatusesAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var overdueSchedules = await _context.PaymentSchedules
            .Where(ps => ps.ScheduledDate < today && 
                        ps.Status == PaymentStatus.Pending)
            .ToListAsync();

        foreach (var schedule in overdueSchedules)
        {
            schedule.Status = PaymentStatus.Overdue;
        }

        if (overdueSchedules.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    #region Private Helper Methods

    private static PaymentScheduleDto MapToDto(PaymentSchedule ps)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var daysUntilDue = ps.ScheduledDate.DayNumber - today.DayNumber;
        var isOverdue = ps.ScheduledDate < today && ps.Status != PaymentStatus.Paid;

        return new PaymentScheduleDto
        {
            Id = ps.Id,
            InvestmentCycleId = ps.InvestmentCycleId,
            ScheduledDate = ps.ScheduledDate,
            ScheduledAmount = ps.ScheduledAmount,
            PaymentType = ps.PaymentType,
            Status = ps.Status,
            PaidAmount = ps.PaidAmount,
            PaidDate = ps.PaidDate,
            Notes = ps.Notes,
            RemainingAmount = ps.ScheduledAmount - ps.PaidAmount,
            IsOverdue = isOverdue,
            DaysUntilDue = daysUntilDue
        };
    }

    #endregion
}
