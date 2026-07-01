using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Enums;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for InvestmentCycle entity
/// </summary>
public class InvestmentCycleConfiguration : IEntityTypeConfiguration<InvestmentCycle>
{
    public void Configure(EntityTypeBuilder<InvestmentCycle> builder)
    {
        // Table name
        builder.ToTable("InvestmentCycles");

        // Primary key
        builder.HasKey(i => i.Id);

        // Properties with decimal precision (18,2 is standard for money)
        builder.Property(i => i.PrincipalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.InterestRate)
            .IsRequired()
            .HasColumnType("decimal(18,4)"); // Higher precision for rates

        builder.Property(i => i.MonthlyInterest)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TotalExpectedProfit)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.FinalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.StartDate)
            .IsRequired();

        builder.Property(i => i.EndDate)
            .IsRequired();

        builder.Property(i => i.DurationMonths)
            .IsRequired();

        builder.Property(i => i.InterestType)
            .IsRequired()
            .HasConversion<int>(); // Store enum as int

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(InvestmentStatus.Active);

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        // Audit fields
        builder.Property(i => i.CreatedDate)
            .IsRequired();

        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(i => i.ParticipantId)
            .HasDatabaseName("IX_InvestmentCycles_ParticipantId");

        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_InvestmentCycles_Status");

        builder.HasIndex(i => i.StartDate)
            .HasDatabaseName("IX_InvestmentCycles_StartDate");

        builder.HasIndex(i => i.EndDate)
            .HasDatabaseName("IX_InvestmentCycles_EndDate");

        builder.HasIndex(i => new { i.ParticipantId, i.Status })
            .HasDatabaseName("IX_InvestmentCycles_ParticipantId_Status");

        // Relationships
        builder.HasOne(i => i.Participant)
            .WithMany(p => p.InvestmentCycles)
            .HasForeignKey(i => i.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.PaymentSchedules)
            .WithOne(ps => ps.InvestmentCycle)
            .HasForeignKey(ps => ps.InvestmentCycleId)
            .OnDelete(DeleteBehavior.Cascade); // Delete schedules when cycle is deleted

        builder.HasMany(i => i.Payments)
            .WithOne(p => p.InvestmentCycle)
            .HasForeignKey(p => p.InvestmentCycleId)
            .OnDelete(DeleteBehavior.Restrict); // Keep payment history
    }
}
