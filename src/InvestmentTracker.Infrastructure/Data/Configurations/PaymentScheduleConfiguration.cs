using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PaymentSchedule entity
/// </summary>
public class PaymentScheduleConfiguration : IEntityTypeConfiguration<PaymentSchedule>
{
    public void Configure(EntityTypeBuilder<PaymentSchedule> builder)
    {
        // Table name
        builder.ToTable("PaymentSchedules");

        // Primary key
        builder.HasKey(ps => ps.Id);

        // Properties
        builder.Property(ps => ps.ScheduledDate)
            .IsRequired();

        builder.Property(ps => ps.ScheduledAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ps => ps.PaidAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(ps => ps.PaymentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ps => ps.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(ps => ps.Notes)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(ps => ps.CreatedDate)
            .IsRequired();

        builder.Property(ps => ps.CreatedBy)
            .HasMaxLength(100);

        builder.Property(ps => ps.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(ps => ps.InvestmentCycleId)
            .HasDatabaseName("IX_PaymentSchedules_InvestmentCycleId");

        builder.HasIndex(ps => ps.ScheduledDate)
            .HasDatabaseName("IX_PaymentSchedules_ScheduledDate");

        builder.HasIndex(ps => ps.Status)
            .HasDatabaseName("IX_PaymentSchedules_Status");

        builder.HasIndex(ps => new { ps.InvestmentCycleId, ps.ScheduledDate })
            .HasDatabaseName("IX_PaymentSchedules_InvestmentCycleId_ScheduledDate");

        // Relationships
        builder.HasOne(ps => ps.InvestmentCycle)
            .WithMany(i => i.PaymentSchedules)
            .HasForeignKey(ps => ps.InvestmentCycleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ps => ps.Payments)
            .WithOne(p => p.PaymentSchedule)
            .HasForeignKey(p => p.PaymentScheduleId)
            .OnDelete(DeleteBehavior.SetNull); // Keep payment but remove link
    }
}
