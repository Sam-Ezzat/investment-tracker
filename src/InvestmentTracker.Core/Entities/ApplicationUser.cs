using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents a user of the application
/// </summary>
public class ApplicationUser : BaseEntity
{
    /// <summary>
    /// Unique username for login
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date and time of the last login
    /// </summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Additional notes about the user
    /// </summary>
    public string? Notes { get; set; }
}
