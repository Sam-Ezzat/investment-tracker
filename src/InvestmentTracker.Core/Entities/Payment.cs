using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents an actual payment transaction
/// </summary>
public class Payment : BaseEntity
{
    /// <summary>
    /// Foreign key to the InvestmentCycle
    /// </summary>
    public int InvestmentCycleId { get; set; }

    /// <summary>
    /// Optional foreign key to the PaymentSchedule (if linked to a schedule)
    /// </summary>
    public int? PaymentScheduleId { get; set; }

    /// <summary>
    /// Amount paid
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Date when the payment was made
    /// </summary>
    public DateOnly PaymentDate { get; set; }

    /// <summary>
    /// Type of payment (Interest, Principal, or Both)
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Method used for payment
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Reference number (cheque number, transfer reference, etc.)
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Name of the person who received the payment
    /// </summary>
    public string? ReceivedBy { get; set; }

    /// <summary>
    /// Additional notes about this payment
    /// </summary>
    public string? Notes { get; set; }

    // Navigation Properties
    /// <summary>
    /// Reference to the investment cycle this payment belongs to
    /// </summary>
    public InvestmentCycle InvestmentCycle { get; set; } = null!;

    /// <summary>
    /// Optional reference to the payment schedule this payment is linked to
    /// </summary>
    public PaymentSchedule? PaymentSchedule { get; set; }
}
