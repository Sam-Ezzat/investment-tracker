namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// DTO for creating a new Participant
/// </summary>
public class CreateParticipantDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
