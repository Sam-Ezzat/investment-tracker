using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for Payment operations
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Get all payments
    /// </summary>
    Task<IEnumerable<PaymentDto>> GetAllAsync();

    /// <summary>
    /// Get payments by investment cycle ID
    /// </summary>
    Task<IEnumerable<PaymentDto>> GetByInvestmentCycleIdAsync(int investmentCycleId);

    /// <summary>
    /// Get payments by date range
    /// </summary>
    Task<IEnumerable<PaymentDto>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Get payment by ID
    /// </summary>
    Task<PaymentDto?> GetByIdAsync(int id);

    /// <summary>
    /// Create a new payment (updates payment schedule if linked)
    /// </summary>
    Task<PaymentDto> CreateAsync(CreatePaymentDto dto);

    /// <summary>
    /// Update an existing payment
    /// </summary>
    Task<PaymentDto> UpdateAsync(int id, CreatePaymentDto dto);

    /// <summary>
    /// Delete a payment
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Get total payments for an investment cycle
    /// </summary>
    Task<decimal> GetTotalPaidAmountAsync(int investmentCycleId);

    /// <summary>
    /// Get recent payments (last N days)
    /// </summary>
    Task<IEnumerable<PaymentDto>> GetRecentAsync(int days = 30);
}
