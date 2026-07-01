using Microsoft.EntityFrameworkCore;
using InvestmentTracker.Core.Entities;
using InvestmentTracker.Core.Interfaces;
using System.Reflection;

namespace InvestmentTracker.Infrastructure.Data;

/// <summary>
/// Application database context for Investment Tracker
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<InvestmentCycle> InvestmentCycles => Set<InvestmentCycle>();
    public DbSet<PaymentSchedule> PaymentSchedules => Set<PaymentSchedule>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Override SaveChanges to automatically populate audit fields
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically populate audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically set audit fields for entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            var currentUser = "System"; // TODO: Get from HttpContext or Identity

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = now;
                entry.Entity.CreatedBy = currentUser;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedDate = now;
                entry.Entity.ModifiedBy = currentUser;
            }
        }
    }
}
