namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents a person who invests money in the system
/// </summary>
public class Participant : BaseEntity
{
    /// <summary>
    /// Full name of the participant
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// National ID or identification number
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateOnly? DateOfBirth { get; set; }

    /// <summary>
    /// Date when the participant registered in the system
    /// </summary>
    public DateOnly RegistrationDate { get; set; }

    /// <summary>
    /// Indicates whether the participant is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional notes about the participant
    /// </summary>
    public string? Notes { get; set; }

    // Navigation Properties
    /// <summary>
    /// Collection of investment cycles for this participant
    /// </summary>
    public ICollection<InvestmentCycle> InvestmentCycles { get; set; } = new List<InvestmentCycle>();
}
