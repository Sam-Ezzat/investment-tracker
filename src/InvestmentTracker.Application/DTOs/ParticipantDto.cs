namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// Data Transfer Object for Participant
/// </summary>
public class ParticipantDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // Summary data
    public int TotalInvestmentCycles { get; set; }
    public decimal TotalInvestedAmount { get; set; }
    public int ActiveCyclesCount { get; set; }
}
