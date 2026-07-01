using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for Participant operations
/// </summary>
public interface IParticipantService
{
    /// <summary>
    /// Get all participants
    /// </summary>
    Task<IEnumerable<ParticipantDto>> GetAllAsync();

    /// <summary>
    /// Get active participants only
    /// </summary>
    Task<IEnumerable<ParticipantDto>> GetActiveAsync();

    /// <summary>
    /// Get participant by ID
    /// </summary>
    Task<ParticipantDto?> GetByIdAsync(int id);

    /// <summary>
    /// Search participants by name, email, or phone
    /// </summary>
    Task<IEnumerable<ParticipantDto>> SearchAsync(string searchTerm);

    /// <summary>
    /// Create a new participant
    /// </summary>
    Task<ParticipantDto> CreateAsync(CreateParticipantDto dto);

    /// <summary>
    /// Update an existing participant
    /// </summary>
    Task<ParticipantDto> UpdateAsync(int id, CreateParticipantDto dto);

    /// <summary>
    /// Delete a participant (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Check if email already exists
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    /// <summary>
    /// Check if national ID already exists
    /// </summary>
    Task<bool> NationalIdExistsAsync(string nationalId, int? excludeId = null);
}
