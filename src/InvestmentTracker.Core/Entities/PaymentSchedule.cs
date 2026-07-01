using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents an automatically generated payment schedule entry
/// </summary>
public class PaymentSchedule : BaseEntity
{
    /// <summary>
    /// Foreign key to the InvestmentCycle
    /// </summary>
    public int InvestmentCycleId { get; set; }

    /// <summary>
    /// The date when payment is scheduled
    /// </summary>
    public DateOnly ScheduledDate { get; set; }

    /// <summary>
    /// The scheduled amount to be paid
    /// </summary>
    public decimal ScheduledAmount { get; set; }

    /// <summary>
    /// Type of payment (Interest, Principal, or Both)
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Current status of this scheduled payment
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// Amount that has been paid towards this schedule
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Date when the payment was completed (if paid)
    /// </summary>
    public DateOnly? PaidDate { get; set; }

    /// <summary>
    /// Additional notes about this scheduled payment
    /// </summary>
    public string? Notes { get; set; }

    // Navigation Properties
    /// <summary>
    /// Reference to the investment cycle this schedule belongs to
    /// </summary>
    public InvestmentCycle InvestmentCycle { get; set; } = null!;

    /// <summary>
    /// Collection of actual payments linked to this schedule
    /// </summary>
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
