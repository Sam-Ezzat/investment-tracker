namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// Data Transfer Object for Setting
/// </summary>
public class SettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public bool IsVisible { get; set; } = true;
    public int DisplayOrder { get; set; }
}
