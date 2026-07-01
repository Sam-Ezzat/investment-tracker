using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for Payment Schedule operations
/// </summary>
public interface IPaymentScheduleService
{
    /// <summary>
    /// Get all payment schedules for an investment cycle
    /// </summary>
    Task<IEnumerable<PaymentScheduleDto>> GetByInvestmentCycleIdAsync(int investmentCycleId);

    /// <summary>
    /// Get payment schedule by ID
    /// </summary>
    Task<PaymentScheduleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Get schedules by status
    /// </summary>
    Task<IEnumerable<PaymentScheduleDto>> GetByStatusAsync(PaymentStatus status);

    /// <summary>
    /// Get overdue payment schedules
    /// </summary>
    Task<IEnumerable<PaymentScheduleDto>> GetOverdueAsync();

    /// <summary>
    /// Get due payment schedules (due within specified days)
    /// </summary>
    Task<IEnumerable<PaymentScheduleDto>> GetDueSoonAsync(int daysAhead = 7);

    /// <summary>
    /// Generate payment schedules for an investment cycle
    /// </summary>
    Task GenerateSchedulesAsync(int investmentCycleId);

    /// <summary>
    /// Update schedule status when payment is made
    /// </summary>
    Task UpdateScheduleStatusAsync(int scheduleId, decimal paidAmount);

    /// <summary>
    /// Update all overdue schedules' statuses
    /// </summary>
    Task UpdateOverdueStatusesAsync();
}
