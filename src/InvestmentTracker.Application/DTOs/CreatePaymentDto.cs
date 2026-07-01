using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// DTO for creating a new Payment
/// </summary>
public class CreatePaymentDto
{
    public int InvestmentCycleId { get; set; }
    public int? PaymentScheduleId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public PaymentType PaymentType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
}
