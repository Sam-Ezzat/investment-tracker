namespace InvestmentTracker.Application.DTOs;

/// <summary>
/// DTO for notification data
/// </summary>
public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // DueToday, DueTomorrow, EndingSoon, Overdue
    public string Icon { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsRead { get; set; }
}

/// <summary>
/// DTO for notification summary
/// </summary>
public class NotificationSummaryDto
{
    public int DueTodayCount { get; set; }
    public int DueTomorrowCount { get; set; }
    public int EndingSoonCount { get; set; }
    public int OverdueCount { get; set; }
    public int TotalUnread { get; set; }
    public List<NotificationDto> RecentNotifications { get; set; } = new();
}
