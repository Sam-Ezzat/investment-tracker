using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents an investment cycle for a participant
/// </summary>
public class InvestmentCycle : BaseEntity
{
    /// <summary>
    /// Foreign key to the Participant
    /// </summary>
    public int ParticipantId { get; set; }

    /// <summary>
    /// The initial amount invested (principal)
    /// </summary>
    public decimal PrincipalAmount { get; set; }

    /// <summary>
    /// Start date of the investment
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Duration of the investment in months
    /// </summary>
    public int DurationMonths { get; set; }

    /// <summary>
    /// Calculated end date of the investment
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Type of interest calculation
    /// </summary>
    public InterestType InterestType { get; set; }

    /// <summary>
    /// Interest rate (percentage or fixed amount depending on InterestType)
    /// </summary>
    public decimal InterestRate { get; set; }

    /// <summary>
    /// Calculated monthly interest amount (if applicable)
    /// </summary>
    public decimal MonthlyInterest { get; set; }

    /// <summary>
    /// Total expected profit at the end of the cycle
    /// </summary>
    public decimal TotalExpectedProfit { get; set; }

    /// <summary>
    /// Final amount (Principal + Total Profit)
    /// </summary>
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// Current status of the investment
    /// </summary>
    public InvestmentStatus Status { get; set; } = InvestmentStatus.Active;

    /// <summary>
    /// Additional notes about this investment cycle
    /// </summary>
    public string? Notes { get; set; }

    // Navigation Properties
    /// <summary>
    /// Reference to the participant who owns this investment
    /// </summary>
    public Participant Participant { get; set; } = null!;

    /// <summary>
    /// Collection of payment schedules for this investment cycle
    /// </summary>
    public ICollection<PaymentSchedule> PaymentSchedules { get; set; } = new List<PaymentSchedule>();

    /// <summary>
    /// Collection of actual payments made for this investment cycle
    /// </summary>
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
