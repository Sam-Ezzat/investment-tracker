namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents the status of an investment cycle
/// </summary>
public enum InvestmentStatus
{
    /// <summary>
    /// Investment is currently active and running
    /// </summary>
    Active = 1,

    /// <summary>
    /// Investment has been completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Investment has been cancelled before completion
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Investment is on hold/suspended
    /// </summary>
    OnHold = 4
}
