using InvestmentTracker.Application.DTOs;

namespace InvestmentTracker.Web.Models.ViewModels;

/// <summary>
/// View model for the dashboard
/// </summary>
public class DashboardViewModel
{
    // Participant Statistics
    public int TotalParticipants { get; set; }
    public int ActiveParticipants { get; set; }

    // Investment Cycle Statistics
    public int TotalInvestmentCycles { get; set; }
    public int ActiveInvestmentCycles { get; set; }
    public int CompletedInvestmentCycles { get; set; }
    public int CyclesEndingSoonCount { get; set; }

    // Financial Statistics
    public decimal TotalInvestedAmount { get; set; }
    public decimal TotalExpectedProfit { get; set; }
    public decimal TotalExpectedReturn { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal TotalRemainingAmount { get; set; }

    // Payment Statistics
    public int RecentPaymentsCount { get; set; }
    public decimal RecentPaymentsTotal { get; set; }
    public int OverduePaymentsCount { get; set; }
    public decimal OverduePaymentsTotal { get; set; }
    public int DueSoonPaymentsCount { get; set; }
    public decimal DueSoonPaymentsTotal { get; set; }
    public int TodaysDueCount { get; set; }
    public decimal TodaysDueTotal { get; set; }
    public decimal MonthlyProfit { get; set; }

    // Recent Data
    public IEnumerable<PaymentDto> RecentPayments { get; set; } = new List<PaymentDto>();
    public IEnumerable<PaymentScheduleDto> OverdueSchedules { get; set; } = new List<PaymentScheduleDto>();
    public IEnumerable<PaymentScheduleDto> DueSoonSchedules { get; set; } = new List<PaymentScheduleDto>();
    public IEnumerable<PaymentScheduleDto> TodaysDueSchedules { get; set; } = new List<PaymentScheduleDto>();
    public IEnumerable<InvestmentCycleDto> CyclesEndingSoon { get; set; } = new List<InvestmentCycleDto>();
    public IEnumerable<ParticipantDto> TopParticipants { get; set; } = new List<ParticipantDto>();

    // Chart Data
    public Dictionary<string, decimal> MonthlyProfitData { get; set; } = new Dictionary<string, decimal>();
    public Dictionary<string, int> CyclesByStatusData { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> PaymentMethodsData { get; set; } = new Dictionary<string, int>();
}
