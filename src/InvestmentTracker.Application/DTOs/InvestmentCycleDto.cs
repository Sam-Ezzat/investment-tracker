using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// Data Transfer Object for Investment Cycle
/// </summary>
public class InvestmentCycleDto
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public decimal PrincipalAmount { get; set; }
    public DateOnly StartDate { get; set; }
    public int DurationMonths { get; set; }
    public DateOnly EndDate { get; set; }
    public InterestType InterestType { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MonthlyInterest { get; set; }
    public decimal TotalExpectedProfit { get; set; }
    public decimal FinalAmount { get; set; }
    public InvestmentStatus Status { get; set; }
    public string? Notes { get; set; }
    
    // Summary data
    public decimal TotalPaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public int DaysRemaining { get; set; }
}
