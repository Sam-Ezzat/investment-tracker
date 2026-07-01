namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents the type of interest calculation for an investment
/// </summary>
public enum InterestType
{
    /// <summary>
    /// Interest is calculated and paid monthly as a percentage
    /// </summary>
    MonthlyPercentage = 1,

    /// <summary>
    /// Fixed profit amount paid at the end of investment period
    /// </summary>
    FixedProfitAtEnd = 2
}
