namespace InvestmentTracker.Core.Interfaces;

/// <summary>
/// Interface for entities that require audit tracking
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    DateTime CreatedDate { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when the entity was last modified
    /// </summary>
    DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// User who last modified the entity
    /// </summary>
    string? ModifiedBy { get; set; }
}
