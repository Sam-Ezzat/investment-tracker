namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents the type of payment being made
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// Payment of interest only
    /// </summary>
    Interest = 1,

    /// <summary>
    /// Payment of principal amount only
    /// </summary>
    Principal = 2,

    /// <summary>
    /// Payment of both principal and interest
    /// </summary>
    PrincipalAndInterest = 3
}
