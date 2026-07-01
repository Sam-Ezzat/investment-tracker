using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Application.DTOs;
using InvestmentTracker.Application.Interfaces;
using InvestmentTracker.Infrastructure.Data;
using System.Globalization;

namespace InvestmentTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for Settings operations
/// </summary>
public class SettingService : ISettingService
{
    private readonly ApplicationDbContext _context;

    public SettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SettingDto>> GetAllAsync()
    {
        return await _context.Settings
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .Select(s => new SettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Category = s.Category,
                Description = s.Description,
                DataType = s.DataType,
                IsVisible = s.IsVisible,
                DisplayOrder = s.DisplayOrder
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SettingDto>> GetByCategoryAsync(string category)
    {
        return await _context.Settings
            .Where(s => s.Category == category)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Category = s.Category,
                Description = s.Description,
                DataType = s.DataType,
                IsVisible = s.IsVisible,
                DisplayOrder = s.DisplayOrder
            })
            .ToListAsync();
    }

    public async Task<SettingDto?> GetByKeyAsync(string key)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
            return null;

        return new SettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.Value,
            Category = setting.Category,
            Description = setting.Description,
            DataType = setting.DataType,
            IsVisible = setting.IsVisible,
            DisplayOrder = setting.DisplayOrder
        };
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        return setting?.Value;
    }

    public async Task<T?> GetValueAsync<T>(string key)
    {
        var value = await GetValueAsync(key);
        
        if (string.IsNullOrEmpty(value))
            return default;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
        catch
        {
            return default;
        }
    }

    public async Task<bool> UpdateValueAsync(string key, string value)
    {
        var setting = await _context.Settings
            .FirstOrDefaultAsync(s => s.Key == key);

        if (setting == null)
            return false;

        setting.Value = value;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateMultipleAsync(Dictionary<string, string> settings)
    {
        foreach (var kvp in settings)
        {
            var setting = await _context.Settings
                .FirstOrDefaultAsync(s => s.Key == kvp.Key);

            if (setting != null)
            {
                setting.Value = kvp.Value;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
