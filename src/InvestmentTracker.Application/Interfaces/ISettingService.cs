using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Application.Interfaces;

/// <summary>
/// Service interface for Settings operations
/// </summary>
public interface ISettingService
{
    /// <summary>
    /// Get all settings
    /// </summary>
    Task<IEnumerable<SettingDto>> GetAllAsync();

    /// <summary>
    /// Get settings by category
    /// </summary>
    Task<IEnumerable<SettingDto>> GetByCategoryAsync(string category);

    /// <summary>
    /// Get setting by key
    /// </summary>
    Task<SettingDto?> GetByKeyAsync(string key);

    /// <summary>
    /// Get setting value as string
    /// </summary>
    Task<string?> GetValueAsync(string key);

    /// <summary>
    /// Get setting value as specific type
    /// </summary>
    Task<T?> GetValueAsync<T>(string key);

    /// <summary>
    /// Update a setting value
    /// </summary>
    Task<bool> UpdateValueAsync(string key, string value);

    /// <summary>
    /// Update multiple settings at once
    /// </summary>
    Task<bool> UpdateMultipleAsync(Dictionary<string, string> settings);
}
