using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment Schedule
/// </summary>
public class PaymentScheduleDto
{
    public int Id { get; set; }
    public int InvestmentCycleId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public decimal ScheduledAmount { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal PaidAmount { get; set; }
    public DateOnly? PaidDate { get; set; }
    public string? Notes { get; set; }
    
    // Calculated fields
    public decimal RemainingAmount { get; set; }
    public bool IsOverdue { get; set; }
    public int DaysUntilDue { get; set; }
}
