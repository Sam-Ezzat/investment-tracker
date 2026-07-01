namespace InvestmentTracker.Core.Enums;

/// <summary>
/// Represents user roles in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator with full access
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Manager with most privileges
    /// </summary>
    Manager = 2,

    /// <summary>
    /// Regular user with standard access
    /// </summary>
    User = 3,

    /// <summary>
    /// Read-only viewer
    /// </summary>
    Viewer = 4
}
