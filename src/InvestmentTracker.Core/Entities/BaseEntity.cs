using InvestmentTracker.Core.Interfaces;

namespace InvestmentTracker.Core.Entities;

/// <summary>
/// Base entity class with common properties for all entities
/// </summary>
public abstract class BaseEntity : IAuditableEntity
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// User who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when the entity was last modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// User who last modified the entity
    /// </summary>
    public string? ModifiedBy { get; set; }
}
