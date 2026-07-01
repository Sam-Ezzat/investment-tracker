using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentTracker.Core.Entities;

namespace InvestmentTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Participant entity
/// </summary>
public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        // Table name
        builder.ToTable("Participants");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Email)
            .HasMaxLength(100);

        builder.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.NationalId)
            .HasMaxLength(50);

        builder.Property(p => p.Address)
            .HasMaxLength(500);

        builder.Property(p => p.RegistrationDate)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        // Audit fields
        builder.Property(p => p.CreatedDate)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(100);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.Email)
            .HasDatabaseName("IX_Participants_Email");

        builder.HasIndex(p => p.NationalId)
            .IsUnique()
            .HasFilter("[NationalId] IS NOT NULL")
            .HasDatabaseName("IX_Participants_NationalId");

        builder.HasIndex(p => p.Phone)
            .HasDatabaseName("IX_Participants_Phone");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Participants_IsActive");

        // Relationships
        builder.HasMany(p => p.InvestmentCycles)
            .WithOne(i => i.Participant)
            .HasForeignKey(i => i.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
    }
}
