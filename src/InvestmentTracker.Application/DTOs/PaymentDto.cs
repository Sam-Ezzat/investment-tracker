using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment
/// </summary>
public class PaymentDto
{
    public int Id { get; set; }
    public int InvestmentCycleId { get; set; }
    public int? PaymentScheduleId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaymentDate { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
    
    // Related data
    public string? ParticipantName { get; set; }
    public string? InvestmentCycleName { get; set; }
}
