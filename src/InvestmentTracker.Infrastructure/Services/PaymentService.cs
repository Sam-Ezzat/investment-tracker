using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Infrastructure.Data;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for Payment operations
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentScheduleService _paymentScheduleService;

    public PaymentService(
        ApplicationDbContext context,
        IPaymentScheduleService paymentScheduleService)
    {
        _context = context;
        _paymentScheduleService = paymentScheduleService;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentDto>> GetByInvestmentCycleIdAsync(int investmentCycleId)
    {
        return await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(p => p.InvestmentCycleId == investmentCycleId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .FirstOrDefaultAsync(p => p.Id == id);

        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentDto dto)
    {
        // Validate investment cycle exists
        var cycleExists = await _context.InvestmentCycles.AnyAsync(ic => ic.Id == dto.InvestmentCycleId);
        if (!cycleExists)
            throw new KeyNotFoundException($"Investment cycle with ID {dto.InvestmentCycleId} not found.");

        // Validate payment schedule if provided
        if (dto.PaymentScheduleId.HasValue)
        {
            var scheduleExists = await _context.PaymentSchedules
                .AnyAsync(ps => ps.Id == dto.PaymentScheduleId.Value);
            if (!scheduleExists)
                throw new KeyNotFoundException($"Payment schedule with ID {dto.PaymentScheduleId} not found.");
        }

        // Create payment
        var payment = new Payment
        {
            InvestmentCycleId = dto.InvestmentCycleId,
            PaymentScheduleId = dto.PaymentScheduleId,
            Amount = dto.Amount,
            PaymentDate = dto.PaymentDate,
            PaymentType = dto.PaymentType,
            PaymentMethod = dto.PaymentMethod,
            ReferenceNumber = dto.ReferenceNumber,
            ReceivedBy = dto.ReceivedBy,
            Notes = dto.Notes
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Update payment schedule if linked
        if (dto.PaymentScheduleId.HasValue)
        {
            await _paymentScheduleService.UpdateScheduleStatusAsync(
                dto.PaymentScheduleId.Value, 
                dto.Amount);
        }

        return (await GetByIdAsync(payment.Id))!;
    }

    public async Task<PaymentDto> UpdateAsync(int id, CreatePaymentDto dto)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            throw new KeyNotFoundException($"Payment with ID {id} not found.");

        // Store old values for schedule update
        var oldScheduleId = payment.PaymentScheduleId;
        var oldAmount = payment.Amount;

        // Update payment
        payment.InvestmentCycleId = dto.InvestmentCycleId;
        payment.PaymentScheduleId = dto.PaymentScheduleId;
        payment.Amount = dto.Amount;
        payment.PaymentDate = dto.PaymentDate;
        payment.PaymentType = dto.PaymentType;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.ReferenceNumber = dto.ReferenceNumber;
        payment.ReceivedBy = dto.ReceivedBy;
        payment.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        // Update payment schedules
        if (oldScheduleId.HasValue && oldScheduleId == dto.PaymentScheduleId)
        {
            // Same schedule - adjust amount
            var amountDifference = dto.Amount - oldAmount;
            await _paymentScheduleService.UpdateScheduleStatusAsync(
                oldScheduleId.Value, 
                amountDifference);
        }
        else
        {
            // Different schedules - reverse old and apply new
            if (oldScheduleId.HasValue)
            {
                await _paymentScheduleService.UpdateScheduleStatusAsync(
                    oldScheduleId.Value, 
                    -oldAmount);
            }
            if (dto.PaymentScheduleId.HasValue)
            {
                await _paymentScheduleService.UpdateScheduleStatusAsync(
                    dto.PaymentScheduleId.Value, 
                    dto.Amount);
            }
        }

        return (await GetByIdAsync(id))!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return false;

        // Reverse payment schedule update if linked
        if (payment.PaymentScheduleId.HasValue)
        {
            await _paymentScheduleService.UpdateScheduleStatusAsync(
                payment.PaymentScheduleId.Value, 
                -payment.Amount);
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<decimal> GetTotalPaidAmountAsync(int investmentCycleId)
    {
        return await _context.Payments
            .Where(p => p.InvestmentCycleId == investmentCycleId)
            .SumAsync(p => p.Amount);
    }

    public async Task<IEnumerable<PaymentDto>> GetRecentAsync(int days = 30)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));

        return await _context.Payments
            .Include(p => p.InvestmentCycle)
                .ThenInclude(ic => ic.Participant)
            .Where(p => p.PaymentDate >= startDate)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    #region Private Helper Methods

    private static PaymentDto MapToDto(Payment p)
    {
        return new PaymentDto
        {
            Id = p.Id,
            InvestmentCycleId = p.InvestmentCycleId,
            PaymentScheduleId = p.PaymentScheduleId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            PaymentType = p.PaymentType,
            PaymentMethod = p.PaymentMethod,
            ReferenceNumber = p.ReferenceNumber,
            ReceivedBy = p.ReceivedBy,
            Notes = p.Notes,
            ParticipantName = p.InvestmentCycle.Participant.FullName,
            InvestmentCycleName = $"{p.InvestmentCycle.Participant.FullName} - {p.InvestmentCycle.StartDate:dd/MM/yyyy}"
        };
    }

    #endregion
}
