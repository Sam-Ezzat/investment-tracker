namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents the status of a scheduled payment
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment is pending and not yet due
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment is due now
    /// </summary>
    Due = 2,

    /// <summary>
    /// Payment has been completed
    /// </summary>
    Paid = 3,

    /// <summary>
    /// Payment is overdue
    /// </summary>
    Overdue = 4,

    /// <summary>
    /// Payment has been partially paid
    /// </summary>
    PartiallyPaid = 5
}
