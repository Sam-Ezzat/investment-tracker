using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Table name
        builder.ToTable("Payments");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.PaymentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.PaymentMethod)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(p => p.ReceivedBy)
            .HasMaxLength(100);

        builder.Property(p => p.Notes)
            .HasMaxLength(500);

        // Audit fields
        builder.Property(p => p.CreatedDate)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.InvestmentCycleId)
            .HasDatabaseName("IX_Payments_InvestmentCycleId");

        builder.HasIndex(p => p.PaymentScheduleId)
            .HasDatabaseName("IX_Payments_PaymentScheduleId");

        builder.HasIndex(p => p.PaymentDate)
            .HasDatabaseName("IX_Payments_PaymentDate");

        builder.HasIndex(p => p.ReferenceNumber)
            .HasDatabaseName("IX_Payments_ReferenceNumber");

        builder.HasIndex(p => new { p.InvestmentCycleId, p.PaymentDate })
            .HasDatabaseName("IX_Payments_InvestmentCycleId_PaymentDate");

        // Relationships
        builder.HasOne(p => p.InvestmentCycle)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.InvestmentCycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PaymentSchedule)
            .WithMany(ps => ps.Payments)
            .HasForeignKey(p => p.PaymentScheduleId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false); // Optional relationship
    }
}
