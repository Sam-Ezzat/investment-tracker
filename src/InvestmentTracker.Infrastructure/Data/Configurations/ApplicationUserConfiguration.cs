using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ApplicationUser entity
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Table name
        builder.ToTable("ApplicationUsers");

        // Primary key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(UserRole.User);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Notes)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(u => u.CreatedDate)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_ApplicationUsers_Username");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_ApplicationUsers_Email");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_ApplicationUsers_IsActive");

        // Seed data - Default Admin User
        builder.HasData(
            new ApplicationUser
            {
                Id = 1,
                Username = "admin",
                Email = "admin@investmenttracker.com",
                // Password: Admin@123 (hashed using BCrypt - this is a placeholder)
                PasswordHash = "$2a$11$vJYHhJvvQPl3/PxKGzHFW.hnJ8X8k8xF.8p0F8M/Z3h6ZrJM8Qq1y",
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System"
            }
        );
    }
}
