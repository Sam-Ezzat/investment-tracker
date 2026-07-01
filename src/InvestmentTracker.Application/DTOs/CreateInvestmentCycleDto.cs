using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// DTO for creating a new Investment Cycle
/// </summary>
public class CreateInvestmentCycleDto
{
    public int ParticipantId { get; set; }
    public decimal PrincipalAmount { get; set; }
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int DurationMonths { get; set; }
    public InterestType InterestType { get; set; }
    public decimal InterestRate { get; set; }
    public string? Notes { get; set; }
}
