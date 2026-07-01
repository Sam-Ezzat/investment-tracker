namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Represents application configuration settings stored in the database
/// </summary>
public class Setting : BaseEntity
{
    /// <summary>
    /// Unique key identifier for the setting
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Value of the setting (stored as string)
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Category or group this setting belongs to
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this setting does
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data type of the value (string, int, bool, decimal, date, etc.)
    /// </summary>
    public string DataType { get; set; } = "string";

    /// <summary>
    /// Indicates whether this setting is visible to users
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Display order for UI presentation
    /// </summary>
    public int DisplayOrder { get; set; }
}
