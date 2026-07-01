namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents the method used for making a payment
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Payment made in cash
    /// </summary>
    Cash = 1,

    /// <summary>
    /// Payment made via bank transfer
    /// </summary>
    BankTransfer = 2,

    /// <summary>
    /// Payment made by cheque
    /// </summary>
    Cheque = 3,

    /// <summary>
    /// Payment made online
    /// </summary>
    Online = 4
}
