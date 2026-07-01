using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for Investment Cycle operations
/// </summary>
public interface IInvestmentCycleService
{
    /// <summary>
    /// Get all investment cycles
    /// </summary>
    Task<IEnumerable<InvestmentCycleDto>> GetAllAsync();

    /// <summary>
    /// Get investment cycles by participant ID
    /// </summary>
    Task<IEnumerable<InvestmentCycleDto>> GetByParticipantIdAsync(int participantId);

    /// <summary>
    /// Get investment cycles by status
    /// </summary>
    Task<IEnumerable<InvestmentCycleDto>> GetByStatusAsync(InvestmentStatus status);

    /// <summary>
    /// Get investment cycle by ID
    /// </summary>
    Task<InvestmentCycleDto?> GetByIdAsync(int id);

    /// <summary>
    /// Create a new investment cycle (calculates all values and generates schedule)
    /// </summary>
    Task<InvestmentCycleDto> CreateAsync(CreateInvestmentCycleDto dto);

    /// <summary>
    /// Update an existing investment cycle
    /// </summary>
    Task<InvestmentCycleDto> UpdateAsync(int id, CreateInvestmentCycleDto dto);

    /// <summary>
    /// Change investment cycle status
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, InvestmentStatus status);

    /// <summary>
    /// Delete an investment cycle
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Calculate investment values (used before saving)
    /// </summary>
    Task<InvestmentCycleDto> CalculateInvestmentAsync(CreateInvestmentCycleDto dto);

    /// <summary>
    /// Get cycles ending soon (within specified days)
    /// </summary>
    Task<IEnumerable<InvestmentCycleDto>> GetEndingSoonAsync(int daysAhead = 30);
}
