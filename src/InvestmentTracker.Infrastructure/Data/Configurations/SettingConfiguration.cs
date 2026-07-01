using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Setting entity
/// </summary>
public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        // Table name
        builder.ToTable("Settings");

        // Primary key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.DataType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("string");

        builder.Property(s => s.IsVisible)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(s => s.CreatedDate)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(s => s.Key)
            .IsUnique()
            .HasDatabaseName("IX_Settings_Key");

        builder.HasIndex(s => s.Category)
            .HasDatabaseName("IX_Settings_Category");

        builder.HasIndex(s => new { s.Category, s.DisplayOrder })
            .HasDatabaseName("IX_Settings_Category_DisplayOrder");

        // Seed default settings
        builder.HasData(
            new Setting
            {
                Id = 1,
                Key = "CompanyName",
                Value = "Investment Tracker",
                Category = "General",
                Description = "Company or application name",
                DataType = "string",
                IsVisible = true,
                DisplayOrder = 1,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 2,
                Key = "Currency",
                Value = "USD",
                Category = "General",
                Description = "Default currency code",
                DataType = "string",
                IsVisible = true,
                DisplayOrder = 2,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 3,
                Key = "CurrencySymbol",
                Value = "$",
                Category = "General",
                Description = "Currency symbol for display",
                DataType = "string",
                IsVisible = true,
                DisplayOrder = 3,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 4,
                Key = "DefaultInterestRate",
                Value = "5.0",
                Category = "Investment",
                Description = "Default monthly interest rate percentage",
                DataType = "decimal",
                IsVisible = true,
                DisplayOrder = 1,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 5,
                Key = "DefaultDurationMonths",
                Value = "12",
                Category = "Investment",
                Description = "Default investment duration in months",
                DataType = "int",
                IsVisible = true,
                DisplayOrder = 2,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 6,
                Key = "EnableEmailNotifications",
                Value = "false",
                Category = "Notifications",
                Description = "Enable email notifications for payments",
                DataType = "bool",
                IsVisible = true,
                DisplayOrder = 1,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 7,
                Key = "ReportLogoPath",
                Value = "/images/logo.png",
                Category = "Reports",
                Description = "Path to logo image for reports",
                DataType = "string",
                IsVisible = true,
                DisplayOrder = 1,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            },
            new Setting
            {
                Id = 8,
                Key = "DateFormat",
                Value = "dd/MM/yyyy",
                Category = "General",
                Description = "Default date format for display",
                DataType = "string",
                IsVisible = true,
                DisplayOrder = 4,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        );
    }
}
